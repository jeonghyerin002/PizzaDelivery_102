using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBoardItemUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI difficultyText;
    public Button acceptButton;

    private StoreSO _data;
    private LocationTrigger _pickupLocation;
    private QuestBoardUI _parentBoard; // 나를 관리하는 보드

    public void Setup(StoreSO data, LocationTrigger pickupLoc, float distance, QuestBoardUI board)
    {
        _data = data;
        _pickupLocation = pickupLoc;
        _parentBoard = board; // 보드 기억하기

        // UI 텍스트 채우기
        if (data.icon != null) iconImage.sprite = data.icon;
        nameText.text = data.questName;
        distanceText.text = $"픽업: {distance:F0}m"; // 소수점 없이 깔끔하게
        rewardText.text = $"{data.reward} G";

        switch (data.difficulty)
        {
            case QuestDifficulty.Easy:
                difficultyText.text = "쉬움";
                difficultyText.color = Color.green; // 초록색
                break;

            case QuestDifficulty.Medium:
                difficultyText.text = "보통";
                difficultyText.color = new Color(1f, 0.6f, 0f); // 주황색 (RGB 커스텀)
                break;

            case QuestDifficulty.Hard:
                difficultyText.text = "어려움";
                difficultyText.color = Color.red; // 빨간색
                break;
        }

        // 버튼 리스너 초기화
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(OnAcceptClicked);
    }

    void OnAcceptClicked()
    {
        // 1. 실제 퀘스트 시작 (매니저에게 알림)
        QuestManager.instance.AcceptQuest(_data, _pickupLocation);

        // 2. 보드에게 "나 빠질게, 새거 채워줘"라고 요청
        if (_parentBoard != null)
        {
            _parentBoard.OnItemAccepted(this.gameObject);
        }
    }
}