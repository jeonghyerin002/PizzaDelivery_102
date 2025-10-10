using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [Header("이 장소에서 받을 수 있는 퀘스트 목록")]
    public List<StoreSO> availableQuests;

    // 플레이어가 이 오브젝트와 상호작용했을 때 호출될 함수
    public void GiveRandomQuest(GameObject player)
    {
        if (availableQuests.Count == 0)
        {
            Debug.Log("제공할 퀘스트가 없습니다.");
            return;
        }

        // 퀘스트 목록에서 무작위로 하나를 선택
        int randomIndex = Random.Range(0, availableQuests.Count);
        StoreSO questToGive = availableQuests[randomIndex];

        // 퀘스트 매니저에게 퀘스트 추가 요청
        QuestManager.instance.AddQuest(questToGive, transform.position);
    }
}