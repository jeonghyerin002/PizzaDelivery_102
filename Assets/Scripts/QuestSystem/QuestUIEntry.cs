using TMPro;
using UnityEngine;
using UnityEngine.UI; // TextMeshPro�� ���ٸ� using TMPro;

public class QuestUIEntry : MonoBehaviour
{
    // ������ ������ UI ��ҵ��� ������ ����
    public TMP_Text questNameText;
    public TMP_Text rewardText;
    public TMP_Text timeLimitText;
    //public Image questIcon;
    public TMP_Text difficultyText;

    private ActiveQuest associatedQuest;

    // ����Ʈ �����͸� �޾� UI�� ä��� �Լ�
    public void Setup(ActiveQuest quest)
    {
        associatedQuest = quest;
        questNameText.text = quest.data.questName;
        rewardText.text = $"����: {quest.data.reward} G";
        //questIcon.sprite = quest.data.icon;

        // ���̵� �ؽ�Ʈ ���� (Enum�� ���ڿ��� ��ȯ)
        difficultyText.text = $"���̵�: {quest.data.difficulty}";

        UpdateTime();
    }

    // ���� �ð�ó�� ��� ���ϴ� ���� ������Ʈ�ϴ� �Լ�
    public void UpdateTime()
    {
        if (associatedQuest != null)
        {
            timeLimitText.text = $"���� �ð�: {(int)associatedQuest.remainingTime}��";
        }
    }

    public ActiveQuest GetAssociatedQuest()
    {
        return associatedQuest;
    }
}