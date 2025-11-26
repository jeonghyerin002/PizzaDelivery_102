using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestUIEntry : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timerText;
    public Slider durabilitySlider;
    public Image durabilityFillImage;

    private ActiveQuest myQuest;
    private QuestMarker myMarker;

    public void Setup(ActiveQuest quest, GameObject markerPrefab, Transform markerParent)
    {
        myQuest = quest;

        if (iconImage != null) iconImage.sprite = quest.data.icon;
        if (titleText != null) titleText.text = quest.data.questName;

        // 마커 생성
        if (markerPrefab != null && markerParent != null)
        {
            GameObject markerObj = Instantiate(markerPrefab, markerParent);
            myMarker = markerObj.GetComponent<QuestMarker>();

            if (myQuest.targetObject != null)
            {
                myMarker.ShowMarker(myQuest.targetObject.transform);
            }
        }

        RefreshDisplay();
    }

    private void Update()
    {
        if (myQuest == null) return;

        UpdateTimerDisplay();
        UpdateDistanceAndStatus();
        UpdateMarkerTarget();
        UpdateDurabilityUI();
    }

    private void UpdateMarkerTarget()
    {
        // 마커가 없거나 퀘스트 목표가 없으면 리턴
        if (myMarker == null || myQuest.targetObject == null) return;

        if (myMarker.target != myQuest.targetObject.transform)
        {
            myMarker.ShowMarker(myQuest.targetObject.transform);
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(myQuest.remainingTime / 60F);
        int seconds = Mathf.FloorToInt(myQuest.remainingTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (myQuest.remainingTime < 10f) timerText.color = Color.red;
        else timerText.color = Color.white;
    }

    private void UpdateDistanceAndStatus()
    {
        if (myQuest.targetObject == null) return;

        float distance = Vector3.Distance(QuestManager.instance.playerTransform.position, myQuest.targetObject.transform.position);

        if (myQuest.state == QuestState.HeadingToPickup)
        {
            statusText.text = $"[픽업] {myQuest.targetObject.name}까지" +
                $"{distance:F0}m";
            statusText.color = Color.yellow;
        }
        else
        {
            statusText.text = $"[배달] {myQuest.targetObject.name}까지" +
                $"{distance:F0}m";
            statusText.color = Color.green;
        }
    }

    private void OnDestroy()
    {
        if (myMarker != null) Destroy(myMarker.gameObject);
    }

    public void RefreshDisplay()
    {
        UpdateTimerDisplay();
        UpdateDistanceAndStatus();
    }

    void UpdateDurabilityUI()
    {
        if (durabilitySlider == null) return;

        if (myQuest.state == QuestState.HeadingToPickup)
        {
            durabilitySlider.gameObject.SetActive(false);
        }
        else
        {
            durabilitySlider.gameObject.SetActive(true);

            float ratio = myQuest.currentDurability / myQuest.data.maxDurability;
            durabilitySlider.value = ratio;
            durabilityFillImage.color = Color.Lerp(Color.red, Color.green, ratio);

        }
    }
}