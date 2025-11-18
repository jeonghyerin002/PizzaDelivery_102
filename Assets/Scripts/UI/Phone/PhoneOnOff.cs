using UnityEngine;
using DG.Tweening;

public class PhoneOnOff : MonoBehaviour
{
    public RectTransform panelRectTransform;

    public float finalAnchorY = 0f;
    public float startAnchorY = -1000f;

    public bool isPhone = false;

    void Start()
    {
        panelRectTransform.gameObject.SetActive(true);
        // 시작할 때 화면 아래로 보내놓기
        panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x, startAnchorY);
        isPhone = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isPhone) ShowPanel();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isPhone) HidePanel();
        }
    }

    public void ShowPanel()
    {
        isPhone = true;
        panelRectTransform.DOKill();
        panelRectTransform.DOAnchorPosY(finalAnchorY, 0.5f).SetEase(Ease.OutBack);
    }

    public void HidePanel()
    {
        isPhone = false;
        panelRectTransform.DOKill();
        panelRectTransform.DOAnchorPosY(startAnchorY, 0.5f).SetEase(Ease.InBack);
    }
}