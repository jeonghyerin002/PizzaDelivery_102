using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyTrigger : MonoBehaviour
{
    public StoreSO emergencyData; // 긴급 퀘스트 데이터
    public LocationTrigger specificPickup; // 특정 픽업지
    public GameObject specificDest; // 특정 도착지

    public void EmergencyQuest()
    {
        QuestManager.instance.TriggerEmergencyQuest(emergencyData, specificPickup, specificDest);
    }
}
