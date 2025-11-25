using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class myDataUI : MonoBehaviour
{
    public TMP_Text deliveryDoneText;
    public TMP_Text deliveryFailText;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        deliveryDoneText.text = $"배달 완료: {myData.DeliveryDone}";
        deliveryFailText.text = $"배달 실패: {myData.DeliveryFail}";
    }
}
