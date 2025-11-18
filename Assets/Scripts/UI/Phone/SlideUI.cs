using UnityEngine;
using DG.Tweening;

public class SlideUI : MonoBehaviour
{
    public float finalAnchorX = 0f;
    public float startAnchorX = 1000f;
    public void ShowPanel(RectTransform targetPanel)
    {
        targetPanel.gameObject.SetActive(true);

        // targetPanel을 기준으로 위치를 잡습니다.
        targetPanel.anchoredPosition = new Vector2(startAnchorX, targetPanel.anchoredPosition.y);

        targetPanel.DOKill(); // 혹시 움직이고 있었다면 멈춤
        targetPanel.DOAnchorPosX(finalAnchorX, 0.4f).SetEase(Ease.OutCubic);
    }
    public void HidePanel(RectTransform targetPanel)
    {
        targetPanel.DOKill();
        targetPanel.DOAnchorPosX(startAnchorX, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            targetPanel.gameObject.SetActive(false);
        });
    }
}