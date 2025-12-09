using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyPlace : MonoBehaviour
{
    public bool isArrive = false;

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
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
                isArrive = true;
                Debug.Log("±ä±Þ Äù½ºÆ® µµÂøÁö °¨ÁöµÊ!");
                QuestManager.instance.OnPlayerReachLocation(quest, null);
            }
        }
    }
}
