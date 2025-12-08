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

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);

        //카메라 뒤쪽에 있는경우
        if (screenPos.z < 0)
        {
            if (markerContainer.gameObject.activeSelf)
                markerContainer.gameObject.SetActive(false);
        }
        else
        {
            if (!markerContainer.gameObject.activeSelf)
                markerContainer.gameObject.SetActive(true);

            markerContainer.position = screenPos;

            //거리 계산
            if (distanceText != null && QuestManager.instance != null)
            {
                float dist = Vector3.Distance(QuestManager.instance.playerTransform.position, target.position);
                distanceText.text = $"{dist:F0}m";
            }
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