using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class myDataUI : MonoBehaviour
{
    public TMP_Text coinText;

    public TMP_Text deliveryDoneText;
    public TMP_Text deliveryFailText;

    public TMP_Text airControlLevelText;
    public TMP_Text maxSwingDistanceLevelText;
    public TMP_Text maxStaminaLevelText;

    private void OnEnable()
    {
        myData.OnDataChanged += UpdateUI;

        UpdateUI();
    }

    private void OnDisable()
    {
        myData.OnDataChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        if (coinText != null)
            coinText.text = $"{myData.Coin}";

        if (deliveryDoneText != null)
            deliveryDoneText.text = $"배달 완료: {myData.DeliveryDone}";

        if (deliveryFailText != null)
            deliveryFailText.text = $"배달 실패: {myData.DeliveryFail}";

        if (airControlLevelText != null)
            airControlLevelText.text = $"Lv.{myData.AirControlLevel}";

        if (maxSwingDistanceLevelText != null)
            maxSwingDistanceLevelText.text = $"Lv.{myData.maxSwingDistanceLevel}";

        if (maxStaminaLevelText != null)
            maxStaminaLevelText.text = $"Lv.{myData.maxStaminaLevel}";
    }
}