using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public Swinging player;

    public TMP_Text airControlCostText;
    public TMP_Text messageText;

    [Header("업그레이드 가격 설정")]
    public int baseAirControlCost = 100;
    public int costIncreasePerLevel = 50;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<Swinging>();

        UpdateUI();
    }

    int GetCurrentCost()
    {
        int currentLevel = myData.AirControlLevel;
        return baseAirControlCost + (currentLevel - 1) * costIncreasePerLevel;
    }

    void UpdateUI()
    {
        int cost = GetCurrentCost();
        airControlCostText.text = $"비용: {cost}";
    }

    public void BuyAirControlUpgrade()
    {
        int cost = GetCurrentCost();

        if (myData.Coin >= cost) // 돈이 충분한지 확인
        {
            myData.Coin -= cost;
            myData.AirControlLevel++;

            player.ApplyStats();

            UpdateUI();

            Debug.Log($"속도 업그레이드 성공! (Lv.{myData.AirControlLevel}) 남은 돈: {myData.Coin}");

            messageText.color = Color.green;
            messageText.text = "업그레이드 성공!";
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
            messageText.color = Color.red;
            messageText.text = "돈이 부족합니다";
        }
    }
}