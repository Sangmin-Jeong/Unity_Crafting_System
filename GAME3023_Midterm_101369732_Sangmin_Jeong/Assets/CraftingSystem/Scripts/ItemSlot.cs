using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;

//Holds reference and count of items, manages their visibility in the Inventory panel
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    public Item _emptyItem;
    public Item item;
    
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI nameText;
    Image itemIcon;
    TextMeshProUGUI itemCountText;

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
    
    // Dragging
    private GameObject _dragingObject;
    [SerializeField] private bool isPicked = false;
    
    // Event
    public EventHandler OnTransferred;
    public EventHandler OnTaken;
    
    void Start()
    {
        Initialize();
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
        //Create instantiated temp object for dragging
        if (TransferManager.Instance._targetSlot)
        {
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
                
                // To transfer whole amount of item that is on output slot
                if (eventData.pointerPress.name == "OutputSlot")
                {
                    TransferManager.Instance._targetSlot.Count += Count;
                    Count = 0;
                    OnTaken?.Invoke(this, EventArgs.Empty);
                    return;
                }
                
                // Check if we dropped the item on the same slot
                if (TransferManager.Instance._targetSlot != this)
                {
                    TransferManager.Instance._targetSlot.Count += 1;
                    Count += -1;
                    OnTransferred?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    TransferManager.Instance._targetSlot.Count += Count;
                }
            }
        }
    }

    void Initialize()
    {
        descriptionText = GameObject.Find("Item Description").GetComponent<TextMeshProUGUI>();
        nameText = GameObject.Find("Item Name").GetComponent<TextMeshProUGUI>();
        itemIcon = transform.Find("Icon").GetComponent<Image>();
        itemCountText = transform.Find("Icon").transform.Find("Count").GetComponent<TextMeshProUGUI>();
    }
}
