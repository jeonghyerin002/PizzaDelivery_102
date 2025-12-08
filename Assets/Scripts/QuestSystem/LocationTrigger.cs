using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum LocationType
{
    Pickup,
    Delivery,
    Both
}

public class LocationTrigger : MonoBehaviour
{
    [Header("장소 설정")]
    public LocationType type;

    public Animator animator;

    private List<ActiveQuest> relatedQuests = new List<ActiveQuest>();

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

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

    public void RemoveQuest(ActiveQuest quest)
    {
        if (relatedQuests.Contains(quest))
        {
            relatedQuests.Remove(quest);
        }
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

                if (quest.state == QuestState.HeadingToPickup && quest.targetObject == this.gameObject)
                {
                    QuestManager.instance.OnPlayerReachLocation(quest, this);

                    if (animator != null) animator.SetTrigger("OnPickup");
                    relatedQuests.Remove(quest);
                }
                else if (quest.state == QuestState.HeadingToDestination && quest.targetObject == this.gameObject)
                {
                    QuestManager.instance.OnPlayerReachLocation(quest, this);

                    if (animator != null)  animator.SetTrigger("OnDelivery");
                    relatedQuests.Remove(quest);
                }
            }
        }
    }
}