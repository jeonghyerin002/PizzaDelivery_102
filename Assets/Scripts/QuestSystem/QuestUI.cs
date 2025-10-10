// QuestUI.cs

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestUI : MonoBehaviour
{
    [Header("UI ������ �� �θ�")]
    public GameObject questEntryPrefab;
    public Transform questListContainer;

    private Dictionary<ActiveQuest, GameObject> spawnedEntries = new Dictionary<ActiveQuest, GameObject>();

    void Start()
    {
        // QuestManager�� �غ�Ǿ����� Ȯ���ϰ� ������ �õ��մϴ�.
        if (QuestManager.instance != null)
        {
            Initialize();
        }
        else
        {
            // ���� �غ���� �ʾҴٸ� 0.1�� �ڿ� �ٽ� �õ��մϴ�.
            // �̴� QuestManager�� ���� �غ�� �ð��� �ִ� ���Դϴ�.
            Invoke(nameof(Initialize), 0.1f);
        }
    }

    void Initialize()
    {
        // ��ȣ(�̺�Ʈ)�� ����(����)�մϴ�.
        QuestManager.instance.onQuestStateChanged += UpdateQuestList;
        // UI�� ���� ����Ʈ ���¿� ����ȭ�ϱ� ���� �� �� ȣ�����ݴϴ�.
        UpdateQuestList();
    }

    // ������Ʈ�� �ı��� �� ��ȣ ������ �����ϰ� �����մϴ�.
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

        // 1. ���� �߰��� ����Ʈ�� UI�� ����
        foreach (var activeQuest in QuestManager.instance.activeQuests)
        {
            if (!spawnedEntries.ContainsKey(activeQuest))
            {
                GameObject newEntry = Instantiate(questEntryPrefab, questListContainer);
                newEntry.GetComponent<QuestUIEntry>().Setup(activeQuest);
                spawnedEntries.Add(activeQuest, newEntry);
            }
        }

        // 2. �Ϸ�ǰų� �����ؼ� ����� ����Ʈ�� UI���� ����
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