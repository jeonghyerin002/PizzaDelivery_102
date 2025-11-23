using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;           //플레이어 체력 (스테미너 관리용)
    public int currentHealth = 100;

    public bool isPickedFood = false;
    public bool isArrivedFood = false;

    void Start()
    {
        currentHealth = maxHealth;
    }


    void Update()
    {
        
    }
}
