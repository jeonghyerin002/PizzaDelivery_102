using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하는 경우

public class BadgeSlotUI : MonoBehaviour
{
    public Image badgeIcon;
    public TextMeshProUGUI badgeName;
    public TextMeshProUGUI badgeDescription;
    private bool privateBadge;
    public void Setup(BadgeData data, bool isUnlocked)
    {
        badgeIcon.sprite = data.badgeIcon;
        badgeName.text = data.badgeName;
        badgeDescription.text = data.badgeDescription;
        privateBadge = data.privateBadge;

        if (isUnlocked)
        {
            badgeIcon.color = Color.white;
        }
        else
        {
            badgeIcon.color = Color.gray;
        }
        if (privateBadge && !isUnlocked)
        {
            badgeName.text = "???";
            badgeDescription.text = "???";
        }
    }
}