using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ���� ���� ���� ����Ʈ�� ���¸� �����ϴ� Ŭ����
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
    public static QuestManager instance; // �̱��� �ν��Ͻ�

    [Header("���̵��� �Ÿ� ����")]
    public float easyMaxDistance = 100f;
    public float mediumMaxDistance = 300f;

    [Header("����Ʈ ����")]
    public List<ActiveQuest> activeQuests = new List<ActiveQuest>();
    private List<QuestDestination> allDestinations = new List<QuestDestination>();

    // UI ������Ʈ�� ���� ��������Ʈ (UI ��ũ��Ʈ���� ����)
    public System.Action onQuestStateChanged;

    private void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // �ʿ� �ִ� ��� �������� �̸� ã�� ����
        allDestinations = FindObjectsOfType<QuestDestination>().ToList();
    }

    private void Update()
    {
        // Ȱ��ȭ�� ����Ʈ���� �ð� ���� ó��
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            activeQuests[i].remainingTime -= Time.deltaTime;
            if (activeQuests[i].remainingTime <= 0)
            {
                Debug.Log($"'{activeQuests[i].data.questName}' ����Ʈ �ð� �ʰ�!");
                activeQuests.RemoveAt(i);
                onQuestStateChanged?.Invoke(); // UI ������Ʈ �˸�
            }
        }
    }

    // ���ο� ����Ʈ�� �߰��ϴ� �Լ�
    public void AddQuest(StoreSO newQuest, Vector3 startPosition)
    {
        GameObject destination = FindDestinationForQuest(newQuest.difficulty, startPosition);

        if (destination != null)
        {
            ActiveQuest quest = new ActiveQuest(newQuest, destination);
            activeQuests.Add(quest);
            Debug.Log($"�ű� ����Ʈ '{newQuest.questName}' ����! ������: {destination.name} ���̵� {newQuest.difficulty}");

            onQuestStateChanged?.Invoke(); // UI ������Ʈ �˸�
        }
        else
        {
            Debug.LogWarning($"{newQuest.difficulty} ���̵��� �´� �������� ã�� �� �����ϴ�.");
        }
    }

    // ����Ʈ �Ϸ� ó�� �Լ�
    public void CompleteQuest(GameObject destinationObject)
    {
        // �ش� �������� ���� ����Ʈ�� ã��
        ActiveQuest questToComplete = activeQuests.FirstOrDefault(q => q.destinationObject == destinationObject);

        if (questToComplete != null)
        {
            Debug.Log($"'{questToComplete.data.questName}' ����Ʈ �Ϸ�! ����: {questToComplete.data.reward}");
            // ���⿡ ���� ���� ���� �߰�
            activeQuests.Remove(questToComplete);
            onQuestStateChanged?.Invoke(); // UI ������Ʈ �˸�
        }
    }

    // ���̵��� ������� ���� ������ �������� ã�� �Լ�
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

        // ��ȿ�� ������ �߿��� �������� �ϳ� ����
        if (validDestinations.Count > 0)
        {
            int randomIndex = Random.Range(0, validDestinations.Count);
            return validDestinations[randomIndex].gameObject;
        }

        return null; // ������ �������� ������ null ��ȯ
    }
}