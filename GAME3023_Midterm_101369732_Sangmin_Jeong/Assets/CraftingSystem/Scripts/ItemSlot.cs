using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;

//Holds reference and count of items, manages their visibility in the Inventory panel
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    public Item _emptyItem;
    public Item item;
    public int Id;

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
    
    // Dragging
    private GameObject _dragingObject;
    [SerializeField] private bool isPicked = false;
    
    // Event
    public EventHandler OnTransferred;
    
    void Start()
    {
        UpdateGraphic();
    }
    private void Update()
    {
        // Dragging object on the mouse position
        if (_dragingObject)
        {
            _dragingObject.transform.position = Input.mousePosition;
        }
    }

    //Change Icon and count
    void UpdateGraphic()
    {
        if (Count < 1)
        {
            item = _emptyItem;
            itemIcon.gameObject.SetActive(false);
            itemCountText.gameObject.SetActive(false);
        }
        else
        {
            //set sprite to the one from the item
            itemIcon.sprite = item.icon;
            itemIcon.gameObject.SetActive(true);
            itemCountText.gameObject.SetActive(true);
            itemCountText.text = Count.ToString();
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
        return (item != null && Count > 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TransferManager.Instance._targetSlot = eventData.pointerEnter.GetComponent<ItemSlot>();
        
        if (item != null)
        {
            descriptionText.text = item.description;
            nameText.text = item.name;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TransferManager.Instance._targetSlot)
        {
            TransferManager.Instance._targetSlot = null;
        }
        
        if(item != null)
        {
            descriptionText.text = "";
            nameText.text = "";
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        //Create instantiated temp object for dragging and disable raycast Target to raycast other objects with mouse.
        if(TransferManager.Instance._targetSlot)
        if (!isPicked && TransferManager.Instance._targetSlot.Count > 0)
        {
            isPicked = true;
            _dragingObject = Instantiate(itemIcon, 
                Input.mousePosition, 
                Quaternion.identity,
                GameObject.Find("Canvas").gameObject.transform).gameObject;
            itemIcon.gameObject.SetActive(false);
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        // Destroy temp object and enable raycast of itemIcon back.
        isPicked = false;
        if (_dragingObject)
        {
            Destroy(_dragingObject);
            if (Count > 1)
            {
                itemIcon.gameObject.SetActive(true);
            }
            
            // Drop Item on ItemSlot and transfer item info
            if (TransferManager.Instance._targetSlot)
            {
                TransferManager.Instance._targetSlot.item = item;
                TransferManager.Instance._targetSlot.Count = Count;
                Count = 0;
                OnTransferred?.Invoke(this, EventArgs.Empty);
            }
            
        }
    }
}
