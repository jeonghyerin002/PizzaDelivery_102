using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BadgeManager : MonoBehaviour
{
    public static BadgeManager Instance;

    [Header("¹îÁö µ¥ÀÌÅÍ ¸®½ºÆ®")]
    public List<BadgeData> allBadges;

    [Header("¹îÁö È¹µæ ¾Ë¸² ÆĞ³Î")]
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

    public void UnlockBadge(int badgeID)
    {
        // ID¿¡ ÇØ´çÇÏ´Â ¹îÁö µ¥ÀÌÅÍ Ã£±â
        BadgeData badgeToUnlock = allBadges.Find(b => b.badgeID == badgeID);

        if (badgeToUnlock != null && !IsBadgeUnlocked(badgeID))
        {
            // PlayerPrefs¿¡ È¹µæ Á¤º¸ ÀúÀå (Key: "Badge_ID", Value: 1)
            PlayerPrefs.SetInt("Badge_" + badgeID, 1);
            PlayerPrefs.Save();

            Debug.Log($"¹îÁö È¹µæ! ID: {badgeID}, ÀÌ¸§: {badgeToUnlock.badgeName}");

            // È¹µæ ¾Ë¸² UI Ç¥½Ã
            ShowBadgeNotification(badgeToUnlock);
        }
    }

    public bool IsBadgeUnlocked(int badgeID)
    {
        // ÀúÀåµÈ °ªÀÌ 1ÀÌ¸é È¹µæÇÑ °Í
        return PlayerPrefs.GetInt("Badge_" + badgeID, 0) == 1;
    }

    //¹îÁö È¹µæ½Ã ui È£Ãâ
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
        int currentDeliveryDone = myData.DeliveryDone;
        if (currentDeliveryDone >= 1)
            UnlockBadge(10);
        if (currentDeliveryDone >= 5)
            UnlockBadge(11);

    }
}