using UnityEngine;

public class Place : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        Arrive();
    }
    public void Arrive()
    {
        ActiveQuest quest = QuestManager.instance.activeQuests.Find(q => q.Destination == this.gameObject);

        if (quest != null)
        {
            if (quest.state == QuestState.HeadingToDestination)
            {
                Debug.Log("±ä±Þ Äù½ºÆ® µµÂøÁö °¨ÁöµÊ!");
                QuestManager.instance.OnPlayerReachLocation(quest, null);
            }
        }
    }
}