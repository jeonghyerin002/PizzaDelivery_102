using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum QuestDifficulty
{
    Easy,
    Medium,
    Hard
}
public enum QuestState
{
    HeadingToPickup,
    HeadingToDestination
}

[CreateAssetMenu(fileName = "New Store", menuName = "Pizza Game/DeliveryItem")]
public class StoreSO : ScriptableObject
{
    public string questName;
    public Sprite icon;          // 물품 사진
    public float pickupTimeLimit; // 픽업하러 갈 때 제한 시간
    public float deliveryTimeLimit; // 배달할 때 제한 시간
    public int reward;
    public QuestDifficulty difficulty;
    public float maxDurability = 100f;
}
