// QuestUI.cs

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestUI : MonoBehaviour
{
    [Header("UI 프리팹 및 부모")]
    public GameObject questEntryPrefab;
    public Transform questListContainer;

    private Dictionary<ActiveQuest, GameObject> spawnedEntries = new Dictionary<ActiveQuest, GameObject>();

    void Start()
    {
        // QuestManager가 준비되었는지 확인하고 연결을 시도합니다.
        if (QuestManager.instance != null)
        {
            Initialize();
        }
        else
        {
            // 아직 준비되지 않았다면 0.1초 뒤에 다시 시도합니다.
            // 이는 QuestManager가 먼저 준비될 시간을 주는 것입니다.
            Invoke(nameof(Initialize), 0.1f);
        }
    }

    void Initialize()
    {
        // 신호(이벤트)를 구독(연결)합니다.
        QuestManager.instance.onQuestStateChanged += UpdateQuestList;
        // UI를 현재 퀘스트 상태와 동기화하기 위해 한 번 호출해줍니다.
        UpdateQuestList();
    }

    // 오브젝트가 파괴될 때 신호 연결을 안전하게 해제합니다.
    void OnDestroy()
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.onQuestStateChanged -= UpdateQuestList;
        }
    }


    private void Update()
    {
        foreach (var entry in spawnedEntries.Values)
        {
            entry.GetComponent<QuestUIEntry>().UpdateTime();
        }
    }

    private void UpdateQuestList()
    {
        if (QuestManager.instance == null) return;

        // 1. 새로 추가된 퀘스트를 UI에 생성
        foreach (var activeQuest in QuestManager.instance.activeQuests)
        {
            if (!spawnedEntries.ContainsKey(activeQuest))
            {
                GameObject newEntry = Instantiate(questEntryPrefab, questListContainer);
                newEntry.GetComponent<QuestUIEntry>().Setup(activeQuest);
                spawnedEntries.Add(activeQuest, newEntry);
            }
        }

        // 2. 완료되거나 실패해서 사라진 퀘스트를 UI에서 제거
        List<ActiveQuest> questsToRemove = spawnedEntries.Keys.Where(q => !QuestManager.instance.activeQuests.Contains(q)).ToList();

        foreach (var quest in questsToRemove)
        {
            if (spawnedEntries[quest] != null)
            {
                Destroy(spawnedEntries[quest]);
            }
            spawnedEntries.Remove(quest);
        }
    }
}