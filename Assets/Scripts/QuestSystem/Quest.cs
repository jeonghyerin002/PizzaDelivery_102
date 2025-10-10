using UnityEngine;

public class Quest
{
    public StoreSO data;          // ���� ����Ʈ ������ (������ ���� ScriptableObject)
    public GameObject destination;  // ���� ��� ���� �� ������ GameObject
    public float remainingTime;     // ���� �ð�

    // ������: ����Ʈ �����Ϳ� �������� �޾� �ʱ�ȭ
    public Quest(StoreSO questData, GameObject dest)
    {
        this.data = questData;
        this.destination = dest;
        this.remainingTime = questData.timeLimit;
    }

    // �� ������ ȣ��Ǿ� �ð��� ���ҽ�Ű�� �Լ�
    public void Tick()
    {
        remainingTime -= Time.deltaTime;
    }
}