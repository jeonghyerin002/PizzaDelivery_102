using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsePopup : MonoBehaviour
{
    public static ItemUsePopup Instance;

    public GameObject popupPanel;
    public Text itemNameText;
    public Image itemIconImage;
    public Button useButton;
    public Button closeButton;

    private ItemSO currentItem;

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
