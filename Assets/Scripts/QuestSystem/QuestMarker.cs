using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestMarker : MonoBehaviour
{
    [Header("UI 연결")]
    public RectTransform markerContainer;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI labelText;
    public Image markerIcon;

    [Header("설정")]
    public Vector3 offset = new Vector3(0, 1.5f, 0); //머리 위 얼마나 띄울지
    public float borderMargin = 20f;    //화면 끝에서 얼마나 안쪽으로 들여보낼지

    public Transform target;
    private Camera mainCamera;
    private bool isActive = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (target == null) HideMarker();
    }

    void Update()
    {
        if (!isActive || target == null) return;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector3 targetPos = target.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPos);

        bool isBehind = screenPos.z < 0;

        Vector2 dir = (Vector2)screenPos - screenCenter;

        if (isBehind)
        {
            dir *= -1;
        }

        float maxX = screenCenter.x - borderMargin;
        float maxY = screenCenter.y - borderMargin;

        // 현재 벡터가 화면 박스를 얼마나 벗어났는지 비율 계산
        float factorX = Mathf.Abs(dir.x) / maxX;
        float factorY = Mathf.Abs(dir.y) / maxY;
        float maxFactor = Mathf.Max(factorX, factorY);

        if (isBehind || maxFactor > 1f)
        {
            dir /= maxFactor;
        }

        // 좌표 적용
        if (!markerContainer.gameObject.activeSelf)
            markerContainer.gameObject.SetActive(true);

        markerContainer.position = screenCenter + dir;

        // 8. 거리 텍스트 업데이트
        if (distanceText != null && QuestManager.instance != null)
        {
            float dist = Vector3.Distance(QuestManager.instance.playerTransform.position, targetPos);
            distanceText.text = $"{dist:F0}m";
        }
    }

    public void ShowMarker(Transform newTarget)
    {
        target = newTarget;
        isActive = true;
        markerContainer.gameObject.SetActive(true);
    }

    public void HideMarker()
    {
        isActive = false;
        if (markerContainer != null) markerContainer.gameObject.SetActive(false);
    }

    public void SetLabel(string text, Color color)
    {
        if(labelText != null)
        {
            labelText.text = text;
            labelText.color = color;
        }

        if (distanceText != null) distanceText.color = color;
    }

    public void SetIcon(Sprite iconSprite)
    {
        if (markerIcon != null && iconSprite != null)
        {
            markerIcon.sprite = iconSprite;
        }
    }
}