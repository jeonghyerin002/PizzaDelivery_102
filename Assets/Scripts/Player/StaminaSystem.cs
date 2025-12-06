using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [Header("스테미나 설정")]
    public float maxStamina = 100f;
    public float regenRate = 10f;       // 초당 회복량
    public float regenDelay = 1.5f;       // 행동 멈춘 후 회복 시작 딜레이

    [Header("UI")]
    public Slider staminaSlider;

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
    }

    // 스테미나 소모 함수 (성공하면 true, 부족하면 false 반환)
    public bool DrainStamina(float amount)
    {
        if (currentStamina <= 0)
        {
            lastActionTime = Time.time; // 시도만 해도 회복 딜레이 갱신
            return false; // 스테미나 부족!
        }

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

    public void ApplyStats()
    {
        float bonus = (myData.maxStaminaLevel - 1) * maxStaminaPerLev;
        maxStamina = maxStamina + bonus;
        staminaSlider.maxValue = maxStamina;

        Debug.Log($"스테미나 {myData.maxStaminaLevel}레벨 적용. 현재 최대 스테미나 {maxStamina}");
    }
}