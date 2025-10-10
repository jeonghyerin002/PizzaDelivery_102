using UnityEngine;

public class Quest
{
    public StoreSO data;          // 원본 퀘스트 데이터 (위에서 만든 ScriptableObject)
    public GameObject destination;  // 실제 배달 가야 할 목적지 GameObject
    public float remainingTime;     // 남은 시간

    // 생성자: 퀘스트 데이터와 목적지를 받아 초기화
    public Quest(StoreSO questData, GameObject dest)
    {
        this.data = questData;
        this.destination = dest;
        this.remainingTime = questData.timeLimit;
    }

    // 매 프레임 호출되어 시간을 감소시키는 함수
    public void Tick()
    {
        remainingTime -= Time.deltaTime;
    }
}