using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestUI : MonoBehaviour
{
    [Header("UI 프리팹")]
    public GameObject questEntryPrefab;
    public GameObject questMarkerPrefab; //마커 프리팹

    [Header("컨테이너")]
    public Transform questListContainer;   // 목록이 쌓일 곳
    public Transform markerContainer;      //마커가 돌아다닐 곳

    private Dictionary<ActiveQuest, QuestUIEntry> spawnedEntries = new Dictionary<ActiveQuest, QuestUIEntry>();

    void Start()
    {
        // QuestManager가 준비 연결
        if (QuestManager.instance != null)
        {
            Initialize();
        }
        else
        {
            Invoke(nameof(Initialize), 0.1f);
        }
    }

    void Initialize()
    {
        QuestManager.instance.onQuestStateChanged += UpdateQuestList;
        UpdateQuestList();
    }

    void OnDestroy()
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.onQuestStateChanged -= UpdateQuestList;
        }
    }

    private void UpdateQuestList()
    {
        if (QuestManager.instance == null) return;

        //퀘스트를 UI에 생성
        foreach (var activeQuest in QuestManager.instance.activeQuests)
        {
            if (!spawnedEntries.ContainsKey(activeQuest))
            {
                GameObject newObj = Instantiate(questEntryPrefab, questListContainer);

                //컴포넌트 생성
                QuestUIEntry newEntry = newObj.GetComponent<QuestUIEntry>();
                newEntry.Setup(activeQuest, questMarkerPrefab, markerContainer);

                spawnedEntries.Add(activeQuest, newEntry);
            }
        }

        //완료되거나 실패한 퀘스트를 UI에서 제거
        List<ActiveQuest> questsToRemove = spawnedEntries.Keys
            .Where(q => !QuestManager.instance.activeQuests.Contains(q))
            .ToList();

        foreach (var quest in questsToRemove)
        {
            if (spawnedEntries[quest] != null)
            {
                Destroy(spawnedEntries[quest].gameObject);
            }
            spawnedEntries.Remove(quest);
        }
    }
}