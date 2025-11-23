using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemSO item;
    public int amount;

    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI amountText;
    public GameObject emptySlotImage;

    public Button slotButton;
    void Start()
    {
        UpdateSlotUI();
        //slotButton.onClick.AddListener(OnSlotClick);
    }
    void OnSlotClick()
    {
        if(item != null)
        {
            ItemUsePopup.Instance.ShowPopup(item, this);
        }
    }


    void Update()
    {
        
    }
    public void SetItem(ItemSO newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
        UpdateSlotUI();

    }

    void UpdateSlotUI()
    {
        if(item != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;

            amountText.text = amount > 1 ? amount.ToString() : "";
            if(emptySlotImage != null)
            {
                emptySlotImage.SetActive(false);
            }
            else
            {
                itemIcon.enabled = false;
                amountText.text = "";
                if(emptySlotImage != null)
                {
                    emptySlotImage.SetActive(true);
                }
            }
        }
    }
    public void AddAmount(int value)
    {
        amount += value;
        UpdateSlotUI();
    }
    public void RemoveAmount(int value)
    {
        amount -= value;

        if(amount <= 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateSlotUI();
        }
    }
    public void ClearSlot()
    {
        item = null;
        amount = 0;
        UpdateSlotUI();
    }
}
