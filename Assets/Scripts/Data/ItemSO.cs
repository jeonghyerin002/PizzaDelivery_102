using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int maxStack = 99;

    public float Timer;

    public bool isUsable = false;
    public int healAmount = 0;

    public enum Item
    { 
        Hambuger,
        Chicken,
        PackedLunch,
        Sushi,
        Jjajangmyeon,
        Pasta,
        Pizza
    
    }

}
