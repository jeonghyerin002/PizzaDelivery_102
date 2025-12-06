using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaSystem : MonoBehaviour
{
    [Header("스테미나 설정")]
    public float maxStamina = 100f;
    public float regenRate = 10f;       // 초당 회복량
    public float regenDelay = 1.5f;       // 행동 멈춘 후 회복 시작 딜레이

    [HideInInspector]
    public bool unlimitedStamina = false;

    [Header("UI")]
    public Slider staminaSlider;
    public GameObject unlimitedPanel;
    public TMP_Text unlimitedTime;
    public float currentTime;

    [Header("업그레이드 설정")]
    public float maxStaminaPerLev = 10f;

    // 현재 스테미나 (외부에서 읽기만 가능)
    public float currentStamina { get; private set; }

    private float lastActionTime;       // 마지막으로 스테미나를 쓴 시간

    void Start()
    {
        ApplyStats();
        currentStamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

    }

    void Update()
    {
        // 3초 딜레이가 지났고 && 스테미나가 꽉 차지 않았다면 -> 회복
        if (Time.time - lastActionTime > regenDelay && currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;

            // 최대치 넘지 않게 고정
            if (currentStamina > maxStamina) currentStamina = maxStamina;

            UpdateUI();
        }

        if (unlimitedPanel.activeSelf)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                StaminaDisplayTime(currentTime);
            }
            else
            {
                currentTime = 0;
                StaminaDisplayTime(0);
            }
        }
    }

    // 스테미나 소모 함수 (성공하면 true, 부족하면 false 반환)
    public bool DrainStamina(float amount)
    {
        if (currentStamina <= 0)
        {
            lastActionTime = Time.time; // 시도만 해도 회복 딜레이 갱신
            return false; // 스테미나 부족!
        }

        if(!unlimitedStamina)
            currentStamina -= amount;

        if (currentStamina < 0) currentStamina = 0;

        lastActionTime = Time.time; // 마지막 행동 시간 갱신
        UpdateUI();

        return true; // 사용 성공
    }

    void UpdateUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }

    public IEnumerator UnlimitedStamina(float duration)
    {
        Debug.Log($"스테미나 {duration}초 동안 무제한");
        unlimitedStamina = true;
        unlimitedPanel.SetActive(true);
        currentTime = duration;

        yield return new WaitForSeconds(duration);
        Debug.Log("스테미나 무제한 끝");
        unlimitedStamina = false;
        unlimitedPanel.SetActive(false);
    }
    void StaminaDisplayTime(float timeToDisplay)
    {
        // 음수가 되지 않도록 보정
        if (timeToDisplay < 0) timeToDisplay = 0;

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (minutes > 0)
        {
            unlimitedTime.text = string.Format("{0}:{1:D2}", minutes, seconds);
        }
        else
        {
            unlimitedTime.text = string.Format("{0:D2}", seconds);
        }
    }

    public void ApplyStats()
    {
        float bonus = (myData.maxStaminaLevel - 1) * maxStaminaPerLev;
        maxStamina = maxStamina + bonus;
        staminaSlider.maxValue = maxStamina;

        Debug.Log($"스테미나 {myData.maxStaminaLevel}레벨 적용. 현재 최대 스테미나 {maxStamina}");
    }
}