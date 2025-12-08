using GLTFast.Schema;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ActiveQuest
{
    public StoreSO data;
    public QuestState state;       // 현재 상태
    public GameObject targetObject; // 현재 가야 할 곳
    public float remainingTime;    // 현재 단계의 남은 시간
    public float currentDurability; //현재 내구도

    public GameObject Destination;

    public ActiveQuest(StoreSO data, GameObject startTarget)
    {
        this.data = data;
        this.state = QuestState.HeadingToPickup;
        this.targetObject = startTarget;
        this.remainingTime = data.pickupTimeLimit;
        this.currentDurability = data.maxDurability;
        this.Destination = null;
    }

    // 픽업 완료 후 배달 단계로 전환하는 함수
    public void SwitchToDeliveryPhase(GameObject finalDestination)
    {
        state = QuestState.HeadingToDestination;
        targetObject = finalDestination;
        remainingTime = data.deliveryTimeLimit;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance; // 싱글톤 인스턴스
    public Transform playerTransform; // 플레이어 위치 계산용
    public QuestBoardUI questBoard;

    [Header("난이도별 픽업지 설정")]
    public float pickupEasyMaxDistance = 70f;
    public float pickupMediumMaxDistance = 150f;
    public float pickupMaxDistance = 200f;

    [Header("난이도별 배달지 설정")]
    public float easyMaxDistance = 100;
    public float mediumMaxDistance = 300;
    public float maxDistance = 400;
    public int maxActiveQuests = 3; //최대 수락 가능 개수

    [Header("퀘스트 관리")]
    public List<ActiveQuest> activeQuests = new List<ActiveQuest>();

    private List<LocationTrigger> pickupLocations = new List<LocationTrigger>();
    private List<LocationTrigger> deliveryLocations = new List<LocationTrigger>();

    public System.Action onQuestStateChanged;

    [HideInInspector]
    public static bool isEmergencyActive = false;

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        // 맵의 모든 위치 후보를 찾음
        var allLocs = FindObjectsOfType<LocationTrigger>();

        pickupLocations = allLocs.Where(x => x.type == LocationType.Pickup || x.type == LocationType.Both).ToList();
        deliveryLocations = allLocs.Where(x => x.type == LocationType.Delivery || x.type == LocationType.Both).ToList();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        GameObject playerobj = GameObject.FindGameObjectWithTag("OriginPlayer");
        playerTransform = playerobj.transform;

        var allLocs = FindObjectsOfType<LocationTrigger>();

        pickupLocations = allLocs.Where(x => x.type == LocationType.Pickup || x.type == LocationType.Both).ToList();
        deliveryLocations = allLocs.Where(x => x.type == LocationType.Delivery || x.type == LocationType.Both).ToList();

        if (questBoard == null)
        {
            questBoard = FindObjectOfType<QuestBoardUI>();
        }
    }


    private void Update()
    {
        if (PauseMenu.isPaused) return;      //게임 일시정지 상태면 return

        // 활성화된 퀘스트들의 시간 감소 처리
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            activeQuests[i].remainingTime -= Time.deltaTime;
            if (activeQuests[i].remainingTime <= 0)
            {
                Debug.Log($"'{activeQuests[i].data.questName}' 퀘스트 시간 초과!");
                myData.DeliveryFail++;
                activeQuests.RemoveAt(i);

                if (activeQuests.Count == 0 && isEmergencyActive)
                {
                    EndEmergencyQuest();
                    Debug.Log("긴급 퀘스트 시간 초과");
                    myData.DeliveryFail++;
                }

                onQuestStateChanged?.Invoke(); // UI 업데이트 알림
            }
        }
    }

    public bool AcceptQuest(StoreSO data, LocationTrigger pickupLocation)
    {
        if (activeQuests.Count >= maxActiveQuests)
        {
            Debug.LogWarning("퀘스트 가방이 모두 찼습니다");
            return false;
        }

        ActiveQuest newQuest = new ActiveQuest(data, pickupLocation.gameObject);
        activeQuests.Add(newQuest);

        // 픽업 장소 활성화 (플레이어가 닿을 수 있게)
        pickupLocation.ActivateTrigger(newQuest);

        Debug.Log($"퀘스트 수락! 픽업지로 이동하세요: {pickupLocation.name}");
        onQuestStateChanged?.Invoke();

        return true;
    }

    public void OnPlayerReachLocation(ActiveQuest quest, LocationTrigger location)
    {
        if (!activeQuests.Contains(quest))
        {
            Debug.LogWarning("이미 종료된 퀘스트");

            // (선택사항) 건물의 기억에서도 지워주면 더 깔끔함
            if (location != null) location.RemoveQuest(quest);

            return;
        }

        if (quest.state == QuestState.HeadingToPickup)
        {
            GameObject finalDest = null;

            if (quest.Destination != null)
            {
                finalDest = quest.Destination;
                Debug.Log("긴급 퀘스트: 지정된 목적지로 설정됨.");
            }
            else
            {
                // 기존 랜덤 로직
                finalDest = FindDestinationForQuest(quest.data.difficulty, location.transform.position);
            }

            if (finalDest != null)
            {
                quest.SwitchToDeliveryPhase(finalDest);

                // 배달지 트리거 활성화
                LocationTrigger destTrigger = finalDest.GetComponent<LocationTrigger>();
                if (destTrigger != null) destTrigger.ActivateTrigger(quest);

                Debug.Log($"물품 픽업 완료! 배달지로 이동하세요: {finalDest.name}");
            }
        }

        else if (quest.state == QuestState.HeadingToDestination)
        {
            Debug.Log($"배달 완료! 보상: {quest.data.reward}");
            activeQuests.Remove(quest);
            myData.DeliveryDone++;
            BadgeManager.Instance.CheckDeliverDone();

            myData.Coin += quest.data.reward;

            // 배달 성공
            if (activeQuests.Count == 0 && isEmergencyActive)
            {
                EndEmergencyQuest();
                Debug.Log("긴급 상황 해제. 정상 영업 재개.");
            }
        }
        onQuestStateChanged?.Invoke();
    }

    public GameObject FindDestinationForQuest(QuestDifficulty difficulty, Vector3 startPosition)
    {
        var potentialDestinations = deliveryLocations
            .Where(loc => Vector3.Distance(startPosition, loc.transform.position) > 20f) // 너무 가까운 곳 제외
            .ToList();

        if (potentialDestinations.Count == 0)
        {
            if (pickupLocations.Count > 0)
            {
                Debug.LogWarning("갈 수 있는 배달지가 없습니다.");
                return pickupLocations[Random.Range(0, pickupLocations.Count)].gameObject;
            }


            return null;
        }

        List<GameObject> validTargets = new List<GameObject>();

        foreach (var dest in potentialDestinations)
        {
            float distance = Vector3.Distance(startPosition, dest.transform.position);
            bool isMatch = false;

            switch (difficulty)
            {
                case QuestDifficulty.Easy: isMatch = distance <= easyMaxDistance; break;
                case QuestDifficulty.Medium: isMatch = distance > easyMaxDistance && distance <= mediumMaxDistance; break;
                case QuestDifficulty.Hard: isMatch = distance > mediumMaxDistance && distance <= maxDistance; break;
            }

            if (isMatch) validTargets.Add(dest.gameObject);
        }

        if (validTargets.Count > 0)
        {
            return validTargets[Random.Range(0, validTargets.Count)];
        }
        return potentialDestinations[Random.Range(0, potentialDestinations.Count)].gameObject;
    }

    public LocationTrigger FindPickupByDifficulty(QuestDifficulty difficulty, Vector3 playerPos)
    {
        if (pickupLocations.Count == 0) return null;

        List<LocationTrigger> validPickups = new List<LocationTrigger>();

        foreach (var shop in pickupLocations)
        {
            float dist = Vector3.Distance(playerPos, shop.transform.position);
            bool isMatch = false;
            

            //픽업 거리 계산
            switch (difficulty)
            {
                case QuestDifficulty.Easy:
                    isMatch = dist <= pickupEasyMaxDistance;
                    break;

                case QuestDifficulty.Medium:
                    isMatch = dist > pickupEasyMaxDistance && dist <= pickupMediumMaxDistance;
                    break;

                case QuestDifficulty.Hard:
                    isMatch = dist > pickupMediumMaxDistance && dist <= pickupMaxDistance;
                    break;
            }

            if (isMatch) validPickups.Add(shop);
        }

        // 조건에 맞는 가게가 있으면 거기서 뽑기
        if (validPickups.Count > 0)
        {
            return validPickups[Random.Range(0, validPickups.Count)];
        }
        //조건에 안맞는게 없으면 랜덤
        Debug.Log("조건에 맞는 픽업지가 없습니다");
        return pickupLocations[Random.Range(0, pickupLocations.Count)];
    }

    public void PackageDamage(float damageAmount)
    {
        bool isChanged = false;

        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            ActiveQuest quest = activeQuests[i];

            if (quest.state == QuestState.HeadingToDestination)
            {
                quest.currentDurability -= damageAmount;
                isChanged = true;

                if(quest.currentDurability <= 0)
                {
                    Debug.Log("퀘스트 실패 물건 파손");
                    if (quest.targetObject != null)
                    {
                        LocationTrigger trigger = quest.targetObject.GetComponent<LocationTrigger>();
                        if (trigger != null) trigger.RemoveQuest(quest);
                    }

                    activeQuests.RemoveAt(i);
                    myData.DeliveryFail++;
                    BadgeManager.Instance.CheckDeliverDone();

                    if (activeQuests.Count == 0 && isEmergencyActive)
                    {
                        Debug.Log("긴급 퀘스트 실패 물건 파손");
                        BadgeManager.Instance.CheckDeliverDone();
                        EndEmergencyQuest();
                    }
                }
            }
        }

        // UI 갱신 알림
        if (isChanged)
        {
            onQuestStateChanged?.Invoke();
        }
    }

    public void TriggerEmergencyQuest(StoreSO questData, LocationTrigger pickupLoc, GameObject fixedDestination)
    {
        //진행중인 퀘스트 삭제
        activeQuests.Clear();

        LocationTrigger[] allTriggers = FindObjectsOfType<LocationTrigger>();
        foreach (var trigger in allTriggers)
        {
            trigger.ResetTrigger();
        }

        isEmergencyActive = true;

        //게시판 잠금
        if (questBoard != null)
            questBoard.SetEmergencyLockdown(true);

        //긴급 퀘스트 생성
        ActiveQuest emergencyQuest = new ActiveQuest(questData, pickupLoc.gameObject);

        //퀘스트 목적 설정
        emergencyQuest.Destination = fixedDestination;

        //리스트에 추가 및 시작
        activeQuests.Add(emergencyQuest);
        pickupLoc.ActivateTrigger(emergencyQuest);

        Debug.LogWarning($"긴급 퀘스트 발동! '{questData.questName}' 시작됨.");
        onQuestStateChanged?.Invoke();
    }
    private void EndEmergencyQuest()
    {
        if (isEmergencyActive)
        {
            isEmergencyActive = false;

            if (questBoard != null)
            {
                questBoard.SetEmergencyLockdown(false); // 잠금 해제
            }

            Debug.Log("긴급 퀘스트 종료");
        }
    }

}