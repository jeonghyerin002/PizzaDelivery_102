using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // ����Ʈ �����ڿ��� ����� ��
        if (other.CompareTag("QuestGiver")) // ���� ������Ʈ�� �±׸� "QuestGiver"�� ����
        {
            QuestGiver questGiver = other.GetComponent<QuestGiver>();
            if (questGiver != null)
            {
                questGiver.GiveRandomQuest(this.gameObject);
            }
        }
        // ����Ʈ �������� ����� ��
        else if (other.CompareTag("QuestDestination")) // ������ ������Ʈ�� �±׸� "QuestDestination"�� ����
        {
            QuestManager.instance.CompleteQuest(other.gameObject);
        }
    }
}