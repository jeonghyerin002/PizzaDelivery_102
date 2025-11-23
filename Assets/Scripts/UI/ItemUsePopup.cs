using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsePopup : MonoBehaviour
{
    public static ItemUsePopup Instance;

    public GameObject popupPanel;
    public TextMeshProUGUI[] itemNameText;         //필요한가..?
    public Image[] itemIconImage;
    //public Button useButton;           아이템을 사용하는 방식이 맞는 건물 위치로 이동하는 거라면 굳이 Use버튼이 패널에 있을 필요가 없음.
    public Button closeButton;

    private ItemSO currentItem;
    private InventorySlot currentSlot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        popupPanel.SetActive(false);
        //useButton.onClick.AddListener(UseItem);
        closeButton.onClick.AddListener(ClosePopup);
    }


    void Update()
    {
        
    }
    public void ShowPopup(ItemSO item, InventorySlot slot)
    {
        

        currentItem = item;
        currentSlot = slot;
        for(int i = 0; i < itemNameText.Length; i++)
        {
            itemNameText[i].text = item.itemName;
        }
        for(int i = 0; i < itemIconImage.Length; i++)
        {
            itemIconImage[i].sprite = item.itemIcon;
        }

        //useButton.interactable = item.isUsable;

        popupPanel.SetActive(true);
    }
    void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
    void UseItem()
    {
        if (currentItem.isUsable)
        {
            //음식 사용 관련 내용 추가
        }
        ClosePopup();
    }
}
