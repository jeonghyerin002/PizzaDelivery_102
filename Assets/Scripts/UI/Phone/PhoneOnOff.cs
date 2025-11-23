using UnityEngine;
using DG.Tweening;

public class PhoneOnOff : MonoBehaviour
{
    public RectTransform panelRectTransform;

    public float finalAnchorY = 0f;
    public float startAnchorY = -1000f;

    public static bool isPhone = false;

    void Start()
    {
        panelRectTransform.gameObject.SetActive(true);
        // 시작할 때 화면 아래로 보내놓기
        panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x, startAnchorY);
        isPhone = false;
    }
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isMoving = h != 0 || v != 0;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isPhone && !isMoving)
            {
                ShowPanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isPhone && !isMoving)
            {
                HidePanel();
            }
        }
    }

    public void ShowPanel()
    {
        Cursor.lockState = CursorLockMode.None;

        isPhone = true;
        panelRectTransform.DOKill();
        panelRectTransform.DOAnchorPosY(finalAnchorY, 0.5f).SetEase(Ease.OutBack);
    }

    public void HidePanel()
    {
        Cursor.lockState = CursorLockMode.Locked;

        isPhone = false;
        panelRectTransform.DOKill();
        panelRectTransform.DOAnchorPosY(startAnchorY, 0.5f).SetEase(Ease.InBack);
    }
}