using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public StoreSO emergencyData; // 긴급 퀘스트 데이터 (폭탄 배달 등)
    public LocationTrigger specificPickup; // 특정 픽업지 (경찰서 등)
    public GameObject specificDest; // 특정 도착지 (테러 현장 등)

    void Update()
    {
        // 예: K키를 누르면 긴급 퀘스트 발동
        if (Input.GetKeyDown(KeyCode.K))
        {
            QuestManager.instance.TriggerEmergencyQuest(emergencyData, specificPickup, specificDest);
        }
    }
}
