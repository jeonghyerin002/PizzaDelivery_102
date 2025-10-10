using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [Header("�� ��ҿ��� ���� �� �ִ� ����Ʈ ���")]
    public List<StoreSO> availableQuests;

    // �÷��̾ �� ������Ʈ�� ��ȣ�ۿ����� �� ȣ��� �Լ�
    public void GiveRandomQuest(GameObject player)
    {
        if (availableQuests.Count == 0)
        {
            Debug.Log("������ ����Ʈ�� �����ϴ�.");
            return;
        }

        // ����Ʈ ��Ͽ��� �������� �ϳ��� ����
        int randomIndex = Random.Range(0, availableQuests.Count);
        StoreSO questToGive = availableQuests[randomIndex];

        // ����Ʈ �Ŵ������� ����Ʈ �߰� ��û
        QuestManager.instance.AddQuest(questToGive, transform.position);
    }
}