using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private int column;
    [SerializeField] private int row;
    private ItemSlot[,] craftingItemSlotsArray;
    List<ItemSlot> craftingItemSlots = new List<ItemSlot>();
    List<ItemSlot> inventoryItemSlots = new List<ItemSlot>();
    private ItemSlot outputSlot;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] GameObject craftingPanel;

    [SerializeField] private Item[] _craftableItems;
    
    void Start()
    {
        outputSlot = GameObject.Find("OutputSlot").GetComponent<ItemSlot>();

        //Read all craftingItemSlots as children of inventory panel
        craftingItemSlots = new List<ItemSlot>(
            craftingPanel.transform.GetComponentsInChildren<ItemSlot>()
        );
        craftingItemSlots.Remove(outputSlot);

        craftingItemSlotsArray = new ItemSlot[row, column];
        
        //Read all inventoryPanel as children of inventory panel
        inventoryItemSlots = new List<ItemSlot>(
            inventoryPanel.transform.GetComponentsInChildren<ItemSlot>()
        );
        
        //Set Slot ID
        int slotCounter = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                craftingItemSlots[slotCounter].OnTransferred += ItemSlot_OnTransferred;
                craftingItemSlotsArray[i, j] = craftingItemSlots[slotCounter++];
            }
            
        }
        // int num = 11;
        // int slotCounter = 0;
        //
        // for (int j = 0; j < row; j++)
        // {
        //     for (int k = 0; k < column; k++)
        //     {
        //         craftingItemSlots[slotCounter].Id = num + k;
        //         craftingItemSlots[slotCounter++].OnTransferred += ItemSlot_OnTransferred;
        //     }
        //     num += 10;
        // }
        
        // Subscribe slot's OnTransferred Event
        foreach (ItemSlot iSlot in inventoryItemSlots)
        {
            iSlot.OnTransferred += ItemSlot_OnTransferred;
        }
    }

    private void ItemSlot_OnTransferred(object sender, EventArgs e)
    {
        CheckRecipes();
    }

    private void CheckRecipes()
    {
        int woodCount = 0;
        int coalCount = 0;
        int stickCount = 0;
        int plankCount = 0;
        
        // Check how many ingredients are on Crafting slots
        foreach (ItemSlot itemSlot in craftingItemSlotsArray)
        {
            if (itemSlot.item)
            {
                if(itemSlot.item.Id == (int)ItemType.WOOD)
                {
                    woodCount++;
                }
                else if (itemSlot.item.Id == (int)ItemType.COAL)
                {
                    coalCount++;
                }
                else if (itemSlot.item.Id == (int)ItemType.STICK)
                {
                    stickCount++;
                }
                else if (itemSlot.item.Id == (int)ItemType.PLANK)
                {
                    plankCount++;
                }
            }
        }
        
        // Crafting slots are empty, no need to check further more
        if (woodCount == 0 && coalCount == 0 && stickCount == 0 && plankCount == 0)
        {
            CleanCraftSlots();
            return;
        }
        
        Debug.Log("Wood: "+woodCount);
        Debug.Log("Coal: "+coalCount);
        Debug.Log("Stick: "+stickCount);
        Debug.Log("Plank: "+plankCount);
        
        // Creafting Slots
        // 11 12 13 14
        // 21 22 23 24
        // 31 32 33 34
        
        // Plank Recipe
        if (woodCount == Plank._requiredAmount)
        {
            Debug.Log("Output: Plank");
            GetItemForOutput(ItemType.PLANK);

        }
        // Stick Recipe
        else if (woodCount == Stick._requiredAmount)
        {
            int slotCounter = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    if (craftingItemSlotsArray[i, j].item)
                    {
                        if (craftingItemSlotsArray[i, j].item.Id == (int)ItemType.WOOD)
                        {
                            if (craftingItemSlotsArray[i + 1, j].item)
                            if (craftingItemSlotsArray[i + 1, j].item.Id == (int)ItemType.WOOD)
                            {
                                Debug.Log("Output: Stick");
                                GetItemForOutput(ItemType.STICK);
                                return;
                            }
                        }
                    }
                }
            
            }
            // foreach (ItemSlot itemSlot in craftingItemSlotsArray)
            // {
            //     if (itemSlot.item)
            //     {
            //         if(itemSlot.item.Id == (int)ItemType.WOOD)
            //         {
            //             
            //             // if (GetCraftingSlotWithID(itemSlot.Id + 10).item.Id == (int)ItemType.WOOD)
            //             // {
            //             //     Debug.Log("Output: Stick");
            //             //     GetItemForOutput(ItemType.STICK);
            //             //     return;
            //             // }
            //             // else
            //             // {
            //             //     return;
            //             // }
            //         }
            //     }
            // }

        }
        else
        {
            outputSlot.Count = 0;
        }
    }

    private void GetItemForOutput(ItemType itemType)
    {
        foreach (Item item in _craftableItems)
        {
            if (item.ItemType == itemType)
            {
                outputSlot.item = item;
                outputSlot.Count = 1;
            }
        }
    }

    ItemSlot GetCraftingSlotWithID(int id)
    {
        foreach (ItemSlot itemSlot in craftingItemSlotsArray)
        {
            if (itemSlot.Id == id)
            {
                return itemSlot;
            }
        }

        return null;
    }

    void CleanCraftSlots()
    {
        foreach (ItemSlot itemSlot in craftingItemSlotsArray)
        {
            itemSlot.Count = 0;
        }

        outputSlot.Count = 0;
    }
}
