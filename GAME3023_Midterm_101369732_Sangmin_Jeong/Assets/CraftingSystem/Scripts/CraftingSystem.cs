using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    List<ItemSlot> itemSlots = new List<ItemSlot>();
    private ItemSlot outputSlot;

    [SerializeField]
    GameObject craftingPanel;

    void Start()
    {
        //Read all itemSlots as children of inventory panel
        itemSlots = new List<ItemSlot>(
            craftingPanel.transform.GetComponentsInChildren<ItemSlot>()
        );

        outputSlot = GameObject.Find("OutputSlot").GetComponent<ItemSlot>();
    }
}
