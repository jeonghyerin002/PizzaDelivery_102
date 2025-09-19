using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Store", menuName = "Pizza Game/Store")]
public class StoreSO : ScriptableObject
{
    [Header("기본 정보")]
    public string StoreName = "가게 이름";
    public StoreType storeType = StoreType.easy;
    

    public enum StoreType
    {
        easy,
        normal,
        hard
    }
}
