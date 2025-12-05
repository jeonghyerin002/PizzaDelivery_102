using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class myData
{
    public static event Action OnDataChanged;

    private const string keyDeliveryDone = "Delivery_Done";
    private const string keyDeliveryFail = "Delivery_Fail";
    private const string keyCoin = "Delivery_Coin";

    public static int DeliveryDone
    {
        get => PlayerPrefs.GetInt(keyDeliveryDone, 0);
        set
        {
            PlayerPrefs.SetInt(keyDeliveryDone, value);
            OnDataChanged?.Invoke();
        }
    }
    public static int DeliveryFail
    {
        get => PlayerPrefs.GetInt(keyDeliveryFail, 0);
        set
        {
            PlayerPrefs.SetInt(keyDeliveryFail, value);
            OnDataChanged?.Invoke();
        }
    }
    public static int Coin
    {
        get => PlayerPrefs.GetInt(keyCoin, 0);
        set
        {
            PlayerPrefs.SetInt(keyCoin, value);
            OnDataChanged?.Invoke();
        }
    }
}
