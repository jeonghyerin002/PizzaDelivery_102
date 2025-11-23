using System.Collections.Generic;
using UnityEngine;

public class QuestBoardUI : MonoBehaviour
{
    [Header("설정")]
    public GameObject questItemPrefab; // 슬롯 프리팹
    public Transform contentTransform;
    public int maxSlots = 10;          // 화면에 띄울 최대 개수

    [Header("데이터")]
    public List<StoreSO> allQuestData;

    private void Start()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            AddRandomQuest();
        }
    }

    public void AddRandomQuest()
    {
        if (allQuestData.Count == 0) return;

        StoreSO randomData = allQuestData[Random.Range(0, allQuestData.Count)];
        LocationTrigger randomPickup = QuestManager.instance.FindRandomPickupLocation();
        if (randomPickup == null) return;

        float dist = Vector3.Distance(QuestManager.instance.playerTransform.position, randomPickup.transform.position);

        GameObject newItem = Instantiate(questItemPrefab, contentTransform);
        QuestBoardItemUI itemUI = newItem.GetComponent<QuestBoardItemUI>();

        itemUI.Setup(randomData, randomPickup, dist, this);
    }

    //아이템 수락
    public void OnItemAccepted(GameObject itemObject)
    {
        //수락된 UI 삭제
        Destroy(itemObject);

        //새로운 퀘스트 하나 채워넣기
        AddRandomQuest();
    }
}