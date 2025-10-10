using TMPro;
using UnityEngine;
using UnityEngine.UI; // TextMeshPro를 쓴다면 using TMPro;

public class QuestUIEntry : MonoBehaviour
{
    // 프리팹 내부의 UI 요소들을 연결할 변수
    public TMP_Text questNameText;
    public TMP_Text rewardText;
    public TMP_Text timeLimitText;
    //public Image questIcon;
    public TMP_Text difficultyText;

    private ActiveQuest associatedQuest;

    // 퀘스트 데이터를 받아 UI를 채우는 함수
    public void Setup(ActiveQuest quest)
    {
        associatedQuest = quest;
        questNameText.text = quest.data.questName;
        rewardText.text = $"보상: {quest.data.reward} G";
        //questIcon.sprite = quest.data.icon;

        // 난이도 텍스트 설정 (Enum을 문자열로 변환)
        difficultyText.text = $"난이도: {quest.data.difficulty}";

        UpdateTime();
    }

    // 남은 시간처럼 계속 변하는 값을 업데이트하는 함수
    public void UpdateTime()
    {
        if (associatedQuest != null)
        {
            timeLimitText.text = $"남은 시간: {(int)associatedQuest.remainingTime}초";
        }
    }

    public ActiveQuest GetAssociatedQuest()
    {
        return associatedQuest;
    }
}