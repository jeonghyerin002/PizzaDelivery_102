using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BadgeManager : MonoBehaviour
{
    public static BadgeManager Instance;

    [Header("뱃지 데이터 리스트")]
    public List<BadgeData> allBadges;

    [Header("뱃지 획득 알림 패널")]
    public GameObject badgeNotificationPanel;
    public Image badgeIconUI;
    public TextMeshProUGUI badgeNameUI;
    public TextMeshProUGUI badgeDescriptionUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetNotificationUI(GameObject panel, Image icon, TextMeshProUGUI name, TextMeshProUGUI desc)
    {
        this.badgeNotificationPanel = panel;
        this.badgeIconUI = icon;
        this.badgeNameUI = name;
        this.badgeDescriptionUI = desc;

        // 연결되자마자 혹시 켜져있을 수 있으니 꺼두기
        if (badgeNotificationPanel != null)
            badgeNotificationPanel.SetActive(false);
    }
    public void UnlockBadge(int badgeID)
    {
        // ID에 해당하는 뱃지 데이터 찾기
        BadgeData badgeToUnlock = allBadges.Find(b => b.badgeID == badgeID);

        if (badgeToUnlock != null && !IsBadgeUnlocked(badgeID))
        {
            // PlayerPrefs에 획득 정보 저장 (Key: "Badge_ID", Value: 1)
            PlayerPrefs.SetInt("Badge_" + badgeID, 1);
            PlayerPrefs.Save();

            Debug.Log($"뱃지 획득! ID: {badgeID}, 이름: {badgeToUnlock.badgeName}");

            // 획득 알림 UI 표시
            ShowBadgeNotification(badgeToUnlock);

            //sfx
            AudioManager.instance.PlaySFX("Challenge");
        }
    }

    public bool IsBadgeUnlocked(int badgeID)
    {
        // 저장된 값이 1이면 획득한 것
        return PlayerPrefs.GetInt("Badge_" + badgeID, 0) == 1;
    }

    //뱃지 획득시 ui 호출
    private void ShowBadgeNotification(BadgeData badgeData)
    {
        badgeIconUI.sprite = badgeData.badgeIcon;
        badgeNameUI.text = badgeData.badgeName;
        badgeDescriptionUI.text = badgeData.badgeDescription;

        badgeNotificationPanel.SetActive(true);

        Invoke(nameof(HideBadgeNotification), 3f);
    }

    private void HideBadgeNotification()
    {
        badgeNotificationPanel.SetActive(false);
    }

    public void CheckDeliverDone()
    {
        int currentDeliveryDone = myData.DeliveryDone;      //배달 데이터 조회
        int currentDeliveryFail = myData.DeliveryFail;

        if (currentDeliveryDone >= 1)
            UnlockBadge(10);
        if (currentDeliveryDone >= 20)
            UnlockBadge(11);
        if (currentDeliveryDone >= 50)
            UnlockBadge(12);

        if (currentDeliveryFail >= 10)
            UnlockBadge(20);
        if (currentDeliveryFail >= 20)
            UnlockBadge(21);
        if (currentDeliveryFail >= 50)
            UnlockBadge(22);
    }
}