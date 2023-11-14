using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cursor = UnityEngine.UIElements.Cursor;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

//Holds reference and count of items, manages their visibility in the Inventory panel
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    public Item item = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField]
    private TMPro.TextMeshProUGUI nameText;

    [SerializeField]
    private int count = 0;
    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            UpdateGraphic();
        }
    }

    [SerializeField]
    Image itemIcon;

    [SerializeField]
    TextMeshProUGUI itemCountText;

    private GameObject DragingObject;
    [SerializeField] private bool isPicked = false; 
    
    void Start()
    {
        UpdateGraphic();
    }

    private void Update()
    {
        if (DragingObject)
        {
            DragingObject.transform.position = Input.mousePosition;
        }
    }

    //Change Icon and count
    void UpdateGraphic()
    {
        if (count < 1)
        {
            item = null;
            itemIcon.gameObject.SetActive(false);
            itemCountText.gameObject.SetActive(false);
        }
        else
        {
            //set sprite to the one from the item
            itemIcon.sprite = item.icon;
            itemIcon.gameObject.SetActive(true);
            itemCountText.gameObject.SetActive(true);
            itemCountText.text = count.ToString();
        }
    }

    public void UseItemInSlot()
    {
        if (CanUseItem())
        {
            item.Use();
            if (item.isConsumable)
            {
                Count--;
            }
        }
    }

    private bool CanUseItem()
    {
        return (item != null && count > 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            descriptionText.text = item.description;
            nameText.text = item.name;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(item != null)
        {
            descriptionText.text = "";
            nameText.text = "";
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Pointer Up");
        isPicked = false;

        if (DragingObject)
        {
            Destroy(DragingObject);
            itemIcon.raycastTarget = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Pointer Down");
        if (!isPicked)
        {
            isPicked = true;
            itemIcon.raycastTarget = false;
            DragingObject = Instantiate(itemIcon, 
                Input.mousePosition, 
                Quaternion.identity, 
                GameObject.Find("Canvas").gameObject.transform).gameObject;
        }
    }
}
