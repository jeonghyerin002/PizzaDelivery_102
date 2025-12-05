using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public Swinging player;

    [Header("UI 연결")]
    public TMP_Text airControlCostText;
    public TMP_Text maxSwingDistanceCostText;
    public TMP_Text messageText;

    [Header("공중 컨트롤 가격")]
    public int baseAirControlCost = 100;

    [Header("스윙 길이 가격")]
    public int baseMaxSwingCost = 100;

    [Header("가격")]
    public int CostIncrease = 50;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<Swinging>();

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

        // 2. MaxSwingDistance 비용 계산
        int swingCost = GetCurrentCost(myData.maxSwingDistanceLevel, baseMaxSwingCost, CostIncrease);
        maxSwingDistanceCostText.text = $"비용: {swingCost}";
    }

    public void BuyAirControlUpgrade()
    {
        int cost = GetCurrentCost(myData.AirControlLevel, baseAirControlCost, CostIncrease);

        if (myData.Coin >= cost) // 돈이 충분한지 확인
        {
            myData.Coin -= cost;
            myData.AirControlLevel++;

            player.ApplyStats();
            UpdateUI();
            ShowMassage("업그레이드 성공!", true);
        }
        else
        {
            ShowMassage("코인이 부족합니다!", false);
        }
    }
    public void BuymaxSwingDistanceUpgrade()
    {
        int cost = GetCurrentCost(myData.maxSwingDistanceLevel, baseMaxSwingCost, CostIncrease);

        if (myData.Coin >= cost) // 돈이 충분한지 확인
        {
            myData.Coin -= cost;
            myData.maxSwingDistanceLevel++;

            player.ApplyStats();
            UpdateUI();
            ShowMassage("업그레이드 성공!", true);
        }
        else
        {
            ShowMassage("코인이 부족합니다!", false);
        }
    }

    void ShowMassage(string msg, bool isSuccess)
    {
        messageText.text = msg;
        messageText.color = isSuccess ? Color.green : Color.red;
    }
}