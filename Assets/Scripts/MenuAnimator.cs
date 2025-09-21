using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuAnimator : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button mainButton;
    [SerializeField] private GameObject buttonContainer;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float targetSpacing = 30f;
    [SerializeField] private float slideDistance = 100f;

    private HorizontalLayoutGroup layoutGroup;
    private CanvasGroup canvasGroup;
    private RectTransform containerRect;
    private Vector2 originalPosition;
    private bool isMenuOpen = false;

    void Start()
    {
        layoutGroup = buttonContainer.GetComponent<HorizontalLayoutGroup>();
        canvasGroup = buttonContainer.GetComponent<CanvasGroup>();
        containerRect = buttonContainer.GetComponent<RectTransform>();
        originalPosition = containerRect.anchoredPosition;

        // ## �߰��� �κ� ##: ������ �� �Ŵ������� �ڽ��� ����մϴ�.
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.RegisterMenu(this);
        }

        // �޴��� ���� ���·� �ʱ�ȭ
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        layoutGroup.spacing = 0;
        containerRect.anchoredPosition = originalPosition - new Vector2(slideDistance, 0);

        mainButton.onClick.AddListener(ToggleMenu);
    }

    // ## ������ �κ� ##: �޴��� ���� �ݴ� ���� ����
    public void ToggleMenu()
    {
        // Case 1: �޴��� �����ִٸ� -> ����, �ٸ� �޴����� ������� �Ŵ������� ��û
        if (!isMenuOpen)
        {
            // �Ŵ������� ���� �����ٰ� �˸�
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.OnMenuOpened(this);
            }
            // ���� �ִϸ��̼� ����
            isMenuOpen = true;
            StopAllCoroutines();
            StartCoroutine(AnimateMenu(true));
        }
        // Case 2: �޴��� �̹� �����ִٸ� -> �׳� �� �޴��� ����
        else
        {
            CloseMenu();
        }
    }

    // ## �߰��� �κ� ##: �Ŵ����� ȣ���� �� �ִ� '�޴� �ݱ�' ���� �Լ�
    public void CloseMenu()
    {
        // �޴��� ���� ���� ���� �ݴ� �ִϸ��̼��� ����
        if (isMenuOpen)
        {
            isMenuOpen = false;
            StopAllCoroutines();
            StartCoroutine(AnimateMenu(false));
        }
    }

    private IEnumerator AnimateMenu(bool open)
    {
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;

        float elapsedTime = 0f;

        float startAlpha = canvasGroup.alpha;
        float endAlpha = open ? 1f : 0f;
        float startSpacing = layoutGroup.spacing;
        float endSpacing = open ? targetSpacing : 0f;

        Vector2 startPosition = containerRect.anchoredPosition;
        Vector2 endPosition = open ? originalPosition : originalPosition - new Vector2(slideDistance, 0);

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            t = t * t * (3f - 2f * t);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            layoutGroup.spacing = Mathf.Lerp(startSpacing, endSpacing, t);
            containerRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        layoutGroup.spacing = endSpacing;
        containerRect.anchoredPosition = endPosition;
    }
}