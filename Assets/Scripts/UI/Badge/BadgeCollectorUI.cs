using UnityEngine;

public class BadgeCollectorUI : MonoBehaviour
{
    public GameObject badgeSlotPrefab; // 뱃지 슬롯 UI 프리팹
    public Transform contentParent;

    void OnEnable()
    {
        UpdateCollectorUI();
    }

    void badgeUpdate()
    {
        UpdateCollectorUI();
    }

    void Start()
    {
        // Start는 다른 스크립트들의 Awake가 끝난 뒤 실행되므로 더 안전함
        UpdateCollectorUI();
    }

    public void UpdateCollectorUI()
    {
        if (BadgeManager.Instance == null)
        {
            Debug.LogWarning("BadgeManager가 아직 준비되지 않았습니다.");
            return;
        }

        // 기존에 생성된 슬롯들 삭제
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // BadgeManager에 있는 모든 뱃지 데이터를 기반으로 슬롯 생성
        foreach (var badgeData in BadgeManager.Instance.allBadges)
        {
            GameObject slotInstance = Instantiate(badgeSlotPrefab, contentParent);
            BadgeSlotUI slotUI = slotInstance.GetComponent<BadgeSlotUI>();

            // 뱃지 획득 여부 확인
            bool isUnlocked = BadgeManager.Instance.IsBadgeUnlocked(badgeData.badgeID);

            // 슬롯 UI 설정
            slotUI.Setup(badgeData, isUnlocked);
        }
    }
}