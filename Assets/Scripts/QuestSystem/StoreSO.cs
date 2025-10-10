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
    [Header("퀘스트 기본 정보")]
    public string questName;        // 퀘스트 이름
    //public Sprite icon;             // UI에 표시될 아이콘
    public QuestDifficulty difficulty; // 난이도
    public int reward;              // 완료 시 보상 (골드 등)
    public float timeLimit;         // 제한 시간 (초 단위)
}
