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

        // ## 추가된 부분 ##: 시작할 때 매니저에게 자신을 등록합니다.
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.RegisterMenu(this);
        }

        // 메뉴를 닫힌 상태로 초기화
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        layoutGroup.spacing = 0;
        containerRect.anchoredPosition = originalPosition - new Vector2(slideDistance, 0);

        mainButton.onClick.AddListener(ToggleMenu);
    }

    // ## 수정된 부분 ##: 메뉴를 열고 닫는 로직 변경
    public void ToggleMenu()
    {
        // Case 1: 메뉴가 닫혀있다면 -> 열고, 다른 메뉴들은 닫으라고 매니저에게 요청
        if (!isMenuOpen)
        {
            // 매니저에게 내가 열린다고 알림
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.OnMenuOpened(this);
            }
            // 열기 애니메이션 실행
            isMenuOpen = true;
            StopAllCoroutines();
            StartCoroutine(AnimateMenu(true));
        }
        // Case 2: 메뉴가 이미 열려있다면 -> 그냥 이 메뉴만 닫음
        else
        {
            CloseMenu();
        }
    }

    // ## 추가된 부분 ##: 매니저가 호출할 수 있는 '메뉴 닫기' 전용 함수
    public void CloseMenu()
    {
        // 메뉴가 열려 있을 때만 닫는 애니메이션을 실행
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