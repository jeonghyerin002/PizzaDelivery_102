using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum QuestDifficulty
{
    Easy,
    Medium,
    Hard
}

[CreateAssetMenu(fileName = "New Store", menuName = "Pizza Game/Store")]
public class StoreSO : ScriptableObject
{
    [Header("����Ʈ �⺻ ����")]
    public string questName;        // ����Ʈ �̸�
    //public Sprite icon;             // UI�� ǥ�õ� ������
    public QuestDifficulty difficulty; // ���̵�
    public int reward;              // �Ϸ� �� ���� (��� ��)
    public float timeLimit;         // ���� �ð� (�� ����)
}
