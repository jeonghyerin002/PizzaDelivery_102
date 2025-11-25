using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ActiveQuest
{
    public StoreSO data;
    public QuestState state;       // 현재 상태
    public GameObject targetObject; // 현재 가야 할 곳
    public float remainingTime;    // 현재 단계의 남은 시간

    public ActiveQuest(StoreSO data, GameObject startTarget)
    {
        this.data = data;
        this.state = QuestState.HeadingToPickup;
        this.targetObject = startTarget;
        this.remainingTime = data.pickupTimeLimit;
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

    [Header("난이도 설정")]
    public float easyMaxDistance = 100f;
    public float mediumMaxDistance = 300f;

    [Header("퀘스트 관리")]
    public List<ActiveQuest> activeQuests = new List<ActiveQuest>();

    private List<LocationTrigger> pickupLocations = new List<LocationTrigger>();
    private List<LocationTrigger> deliveryLocations = new List<LocationTrigger>();

    public System.Action onQuestStateChanged;

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        // 맵의 모든 위치 후보를 찾음
        var allLocs = FindObjectsOfType<LocationTrigger>();

        pickupLocations = allLocs.Where(x => x.type == LocationType.Pickup).ToList();
        deliveryLocations = allLocs.Where(x => x.type == LocationType.Delivery).ToList();
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
                onQuestStateChanged?.Invoke(); // UI 업데이트 알림
            }
        }
    }

    public void AcceptQuest(StoreSO data, LocationTrigger pickupLocation)
    {
        ActiveQuest newQuest = new ActiveQuest(data, pickupLocation.gameObject);
        activeQuests.Add(newQuest);

        // 픽업 장소 활성화 (플레이어가 닿을 수 있게)
        pickupLocation.ActivateTrigger(newQuest);

        Debug.Log($"퀘스트 수락! 픽업지로 이동하세요: {pickupLocation.name}");
        onQuestStateChanged?.Invoke();
    }

    public void OnPlayerReachLocation(ActiveQuest quest, LocationTrigger location)
    {
        if (quest.state == QuestState.HeadingToPickup)
        {
            GameObject finalDest = FindDestinationForQuest(quest.data.difficulty, location.transform.position);

            if (finalDest != null)
            {
                quest.SwitchToDeliveryPhase(finalDest);

                // 배달지 트리거 활성화
                LocationTrigger destTrigger = finalDest.GetComponent<LocationTrigger>();
                if (destTrigger != null) destTrigger.ActivateTrigger(quest);

                Debug.Log($"물품 픽업 완료! 배달지로 이동하세요: {finalDest.name}");
            }
            else
            {
                Debug.LogError("적절한 배달지를 찾지 못했습니다.");
            }
        }
        else if (quest.state == QuestState.HeadingToDestination)
        {
            // 배달 성공
            myData.DeliveryDone++;
            Debug.Log($"배달 완료! 보상: {quest.data.reward}");
            activeQuests.Remove(quest);
        }

        onQuestStateChanged?.Invoke();
    }

    public GameObject FindDestinationForQuest(QuestDifficulty difficulty, Vector3 startPosition)
    {
        var potentialDestinations = deliveryLocations
            .Where(loc => Vector3.Distance(startPosition, loc.transform.position) > 10f) // 너무 가까운 곳 제외
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
                case QuestDifficulty.Hard: isMatch = distance > mediumMaxDistance; break;
            }

            if (isMatch) validTargets.Add(dest.gameObject);
        }

        if (validTargets.Count > 0)
        {
            return validTargets[Random.Range(0, validTargets.Count)];
        }
        return potentialDestinations[Random.Range(0, potentialDestinations.Count)].gameObject;
    }

    public LocationTrigger FindRandomPickupLocation()
    {
        // [안전장치] 만약 맵에 'Pickup' 타입의 건물이 하나도 없다면 에러 방지
        if (pickupLocations.Count == 0)
        {
            Debug.LogError("PicUp LocationTrigger가 한 개도 없습니다!");
            return null;
        }

        // 0부터 리스트 크기(개수) 사이의 랜덤 숫자를 뽑음
        int randomIndex = Random.Range(0, pickupLocations.Count);

        // 해당 번호의 장소를 반환
        return pickupLocations[randomIndex];
    }
}