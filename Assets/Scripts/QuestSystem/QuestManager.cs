using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 현재 진행 중인 퀘스트의 상태를 저장하는 클래스
public class ActiveQuest
{
    public StoreSO data;
    public GameObject destinationObject;
    public float remainingTime;
    public QuestDifficulty difficulty;

    public ActiveQuest(StoreSO questData, GameObject dest)
    {
        data = questData;
        destinationObject = dest;
        remainingTime = questData.timeLimit;
        difficulty = questData.difficulty;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance; // 싱글톤 인스턴스

    [Header("난이도별 거리 설정")]
    public float easyMaxDistance = 100f;
    public float mediumMaxDistance = 300f;

    [Header("퀘스트 관리")]
    public List<ActiveQuest> activeQuests = new List<ActiveQuest>();
    private List<QuestDestination> allDestinations = new List<QuestDestination>();

    // UI 업데이트를 위한 델리게이트 (UI 스크립트에서 구독)
    public System.Action onQuestStateChanged;

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 맵에 있는 모든 목적지를 미리 찾아 저장
        allDestinations = FindObjectsOfType<QuestDestination>().ToList();
    }

    private void Update()
    {
        // 활성화된 퀘스트들의 시간 감소 처리
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            activeQuests[i].remainingTime -= Time.deltaTime;
            if (activeQuests[i].remainingTime <= 0)
            {
                Debug.Log($"'{activeQuests[i].data.questName}' 퀘스트 시간 초과!");
                activeQuests.RemoveAt(i);
                onQuestStateChanged?.Invoke(); // UI 업데이트 알림
            }
        }
    }

    // 새로운 퀘스트를 추가하는 함수
    public void AddQuest(StoreSO newQuest, Vector3 startPosition)
    {
        GameObject destination = FindDestinationForQuest(newQuest.difficulty, startPosition);

        if (destination != null)
        {
            ActiveQuest quest = new ActiveQuest(newQuest, destination);
            activeQuests.Add(quest);
            Debug.Log($"신규 퀘스트 '{newQuest.questName}' 수락! 목적지: {destination.name} 난이도 {newQuest.difficulty}");

            onQuestStateChanged?.Invoke(); // UI 업데이트 알림
        }
        else
        {
            Debug.LogWarning($"{newQuest.difficulty} 난이도에 맞는 목적지를 찾을 수 없습니다.");
        }
    }

    // 퀘스트 완료 처리 함수
    public void CompleteQuest(GameObject destinationObject)
    {
        // 해당 목적지를 가진 퀘스트를 찾음
        ActiveQuest questToComplete = activeQuests.FirstOrDefault(q => q.destinationObject == destinationObject);

        if (questToComplete != null)
        {
            Debug.Log($"'{questToComplete.data.questName}' 퀘스트 완료! 보상: {questToComplete.data.reward}");
            // 여기에 보상 지급 로직 추가
            activeQuests.Remove(questToComplete);
            onQuestStateChanged?.Invoke(); // UI 업데이트 알림
        }
    }

    // 난이도와 출발지에 따라 적절한 목적지를 찾는 함수
    private GameObject FindDestinationForQuest(QuestDifficulty difficulty, Vector3 startPosition)
    {
        List<QuestDestination> validDestinations = new List<QuestDestination>();

        foreach (var dest in allDestinations)
        {
            float distance = Vector3.Distance(startPosition, dest.transform.position);

            if (difficulty == QuestDifficulty.Easy && distance <= easyMaxDistance)
            {
                validDestinations.Add(dest);
            }
            else if (difficulty == QuestDifficulty.Medium && distance > easyMaxDistance && distance <= mediumMaxDistance)
            {
                validDestinations.Add(dest);
            }
            else if (difficulty == QuestDifficulty.Hard && distance > mediumMaxDistance)
            {
                validDestinations.Add(dest);
            }
        }

        // 유효한 목적지 중에서 랜덤으로 하나 선택
        if (validDestinations.Count > 0)
        {
            int randomIndex = Random.Range(0, validDestinations.Count);
            return validDestinations[randomIndex].gameObject;
        }

        return null; // 적절한 목적지가 없으면 null 반환
    }
}