using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 퀘스트 제공자에게 닿았을 때
        if (other.CompareTag("QuestGiver")) // 가게 오브젝트의 태그를 "QuestGiver"로 설정
        {
            QuestGiver questGiver = other.GetComponent<QuestGiver>();
            if (questGiver != null)
            {
                questGiver.GiveRandomQuest(this.gameObject);
            }
        }
        // 퀘스트 목적지에 닿았을 때
        else if (other.CompareTag("QuestDestination")) // 목적지 오브젝트의 태그를 "QuestDestination"로 설정
        {
            QuestManager.instance.CompleteQuest(other.gameObject);
        }
    }
}