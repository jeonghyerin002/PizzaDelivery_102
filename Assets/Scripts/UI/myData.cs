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

    //½ºÅÈ
    private const string keyStaminaLevel = "Stat_StaminaLevel3";
    private const string keyAirControlLevel = "Stat_AirControlLevel3";
    private const string keymaxSwingDistanceLevel = "Stat_maxSwingDistance3";
    private const string keymaxStamina = "Stat_maxStamina3";



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

    //½ºÅÈ
    public static int StaminaLevel
    {
        get => PlayerPrefs.GetInt(keyStaminaLevel, 1);
        set
        {
            PlayerPrefs.SetInt(keyStaminaLevel, value);
            OnDataChanged?.Invoke();
        }
    }
    public static int AirControlLevel
    {
        get => PlayerPrefs.GetInt(keyAirControlLevel, 1);
        set
        {
            PlayerPrefs.SetInt(keyAirControlLevel, value);
            OnDataChanged?.Invoke();
        }
    }

    public static int maxSwingDistanceLevel
    {
        get => PlayerPrefs.GetInt(keymaxSwingDistanceLevel, 1);
        set
        {
            PlayerPrefs.SetInt(keymaxSwingDistanceLevel, value);
            OnDataChanged?.Invoke();
        }
    }
    public static int maxStaminaLevel
    {
        get => PlayerPrefs.GetInt(keymaxStamina, 1);
        set
        {
            PlayerPrefs.SetInt(keymaxStamina, value);
            OnDataChanged?.Invoke();
        }
    }

}
