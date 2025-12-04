using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum LocationType
{
    Pickup,
    Delivery
}

public class LocationTrigger : MonoBehaviour
{
    [Header("장소 설정")]
    public LocationType type;

    private List<ActiveQuest> relatedQuests = new List<ActiveQuest>();

    public void ActivateTrigger(ActiveQuest quest)
    {
        if (!relatedQuests.Contains(quest))
        {
            relatedQuests.Add(quest);
        }
    }

    public void ResetTrigger()
    {
        relatedQuests.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && relatedQuests.Count > 0)
        {
            var questsToProcess = new List<ActiveQuest>(relatedQuests);

            foreach (var quest in questsToProcess)
            {
                bool isPickupMatch = (quest.state == QuestState.HeadingToPickup && quest.targetObject == this.gameObject);
                bool isDeliveryMatch = (quest.state == QuestState.HeadingToDestination && quest.targetObject == this.gameObject);

                if (isPickupMatch || isDeliveryMatch)
                {
                    QuestManager.instance.OnPlayerReachLocation(quest, this);
                    relatedQuests.Remove(quest);
                }
            }
        }
    }
}