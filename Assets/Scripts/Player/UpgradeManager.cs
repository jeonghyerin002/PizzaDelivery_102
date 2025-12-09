using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public Swinging swinging;
    public StaminaSystem staminaSystem;

    [Header("UI 연결")]
    public TMP_Text airControlCostText;
    public TMP_Text maxSwingDistanceCostText;
    public TMP_Text maxStaminaText;
    public TMP_Text messageText;

    [Header("가격")]
    public int baseAirControlCost = 100;
    public int baseMaxSwingCost = 100;
    public int baseMaxStaminaCost = 100;

    [Header("가격 상승")]
    public int CostIncrease = 50;

    void Start()
    {
        if (swinging == null)
            swinging = FindObjectOfType<Swinging>();

        if(staminaSystem == null)
            staminaSystem = FindObjectOfType<StaminaSystem>();

        UpdateUI();
    }

    int GetCurrentCost(int currentLevel, int baseCost, int perLevelIncrease)
    {
        return baseCost + (currentLevel - 1) * perLevelIncrease;
    }

    void UpdateUI()
    {
        int airCost = GetCurrentCost(myData.AirControlLevel, baseAirControlCost, CostIncrease);
        airControlCostText.text = $"비용: {airCost}";


        int swingCost = GetCurrentCost(myData.maxSwingDistanceLevel, baseMaxSwingCost, CostIncrease);
        maxSwingDistanceCostText.text = $"비용: {swingCost}";

        int staminaCost = GetCurrentCost(myData.maxStaminaLevel, baseMaxStaminaCost, CostIncrease);
        maxStaminaText.text = $"비용: {staminaCost}";
    }

    public void BuyAirControlUpgrade()
    {
        int cost = GetCurrentCost(myData.AirControlLevel, baseAirControlCost, CostIncrease);

        if (myData.maxSwingDistanceLevel >= 10)
        {
            ShowMassage("더이상 업그레이드 할 수 없습니다!", false);
            AudioManager.instance.PlaySFX("Fail");
            return;
        }
        else if (myData.Coin >= cost) // 돈이 충분한지 확인
        {
            myData.Coin -= cost;
            myData.AirControlLevel++;

            swinging.ApplyStats();
            UpdateUI();
            ShowMassage("업그레이드 성공!", true);
            AudioManager.instance.PlaySFX("Done");
        }
        else
        {
            ShowMassage("코인이 부족합니다!", false);
            AudioManager.instance.PlaySFX("Fail");
        }
    }
    public void BuymaxSwingDistanceUpgrade()
    {
        int cost = GetCurrentCost(myData.maxSwingDistanceLevel, baseMaxSwingCost, CostIncrease);

        if(myData.maxSwingDistanceLevel >= 10)
        {
            ShowMassage("더이상 업그레이드 할 수 없습니다!", false);
            AudioManager.instance.PlaySFX("Fail");
            return;
        }
        else if (myData.Coin >= cost) // 돈이 충분한지 확인
        {
            myData.Coin -= cost;
            myData.maxSwingDistanceLevel++;

            swinging.ApplyStats();
            UpdateUI();
            ShowMassage("업그레이드 성공!", true);
            AudioManager.instance.PlaySFX("Done");
        }
        else
        {
            ShowMassage("코인이 부족합니다!", false);
            AudioManager.instance.PlaySFX("Fail");
        }
    }
    public void BuymaxStaminaUpgrade()
    {
        int cost = GetCurrentCost(myData.maxStaminaLevel, baseMaxStaminaCost, CostIncrease);

        if (myData.maxSwingDistanceLevel >= 10)
        {
            ShowMassage("더이상 업그레이드 할 수 없습니다!", false);
            AudioManager.instance.PlaySFX("Fail");
            return;
        }
        else if (myData.Coin >= cost) // 돈이 충분한지 확인
        {
            myData.Coin -= cost;
            myData.maxStaminaLevel++;

            staminaSystem.ApplyStats();
            UpdateUI();
            ShowMassage("업그레이드 성공!", true);
            AudioManager.instance.PlaySFX("Done");
        }
        else
        {
            ShowMassage("코인이 부족합니다!", false);
            AudioManager.instance.PlaySFX("Fail");
        }
    }

    void ShowMassage(string msg, bool isSuccess)
    {
        messageText.text = msg;
        messageText.color = isSuccess ? Color.green : Color.red;
    }
}