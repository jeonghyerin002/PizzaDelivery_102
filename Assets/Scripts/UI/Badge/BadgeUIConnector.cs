using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BadgeUIConnector : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject badgeNotificationPanel;
    public Image badgeIconUI;
    public TextMeshProUGUI badgeNameUI;
    public TextMeshProUGUI badgeDescriptionUI;

    private void Start()
    {
        // 씬이 시작될 때, 살아있는 BadgeManager 싱글톤에 현재 씬의 UI를 연결해줍니다.
        if (BadgeManager.Instance != null)
        {
            BadgeManager.Instance.SetNotificationUI(
                badgeNotificationPanel,
                badgeIconUI,
                badgeNameUI,
                badgeDescriptionUI
            );
        }
    }
}