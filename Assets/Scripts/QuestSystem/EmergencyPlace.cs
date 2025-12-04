using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyPlace : MonoBehaviour
{    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("플레이어 감지");
            Arrive();
        }

    }
    public void Arrive()
    {
        ActiveQuest quest = QuestManager.instance.activeQuests.Find(q => q.Destination == this.gameObject);

        if (quest != null)
        {
            if (quest.state == QuestState.HeadingToDestination)
            {
                Debug.Log("긴급 퀘스트 도착지 감지됨!");
                QuestManager.instance.OnPlayerReachLocation(quest, null);
            }
        }
    }
}
