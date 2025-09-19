using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Store", menuName = "Pizza Game/Store")]
public class StoreSO : ScriptableObject
{
    [Header("�⺻ ����")]
    public string StoreName = "���� �̸�";
    public StoreType storeType = StoreType.easy;
    

    public enum StoreType
    {
        easy,
        normal,
        hard
    }
}
