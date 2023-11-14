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
        int emptyCount = 0;
        
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
                else if (itemSlot.item.Id == (int)ItemType.EMPTY)
                {
                    emptyCount++;
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
        Debug.Log("Plank: "+emptyCount);
        
        // Creafting Slots
        // 11 12 13 14
        // 21 22 23 24
        // 31 32 33 34
        
        // Plank Recipe
        // Only need to check Amount of wood on Crafting slots
        if (woodCount == Plank._requiredAmount)
        {
            Debug.Log("Output: Plank");
            GetItemForOutput(ItemType.PLANK, 4);

        }
        // Stick Recipe
        else if (plankCount == Stick._requiredAmount)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    // Check if there is the same permutation like Stick's recipe 
                    if(i + 1 < row)
                    if (craftingItemSlotsArray[i, j].item.Id == Stick._recipe[0,0] &&
                        craftingItemSlotsArray[i + 1, j].item.Id == Stick._recipe[1, 0])
                    {
                        Debug.Log("Output: Stick");
                        GetItemForOutput(ItemType.STICK, 4);
                        return;
                    }
                }
                
            }
            CleanOutPutSlot();
        }
                // Stick Recipe
        else if (plankCount == Stick._requiredAmount)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    // Check if there is the same permutation like Stick's recipe 
                    if(i + 1 < row)
                    if (craftingItemSlotsArray[i, j].item.Id == Stick._recipe[0,0] &&
                        craftingItemSlotsArray[i + 1, j].item.Id == Stick._recipe[1, 0])
                    {
                        Debug.Log("Output: Stick");
                        GetItemForOutput(ItemType.STICK, 4);
                        return;
                    }
                }
                
            }
            CleanOutPutSlot();
        }
        else
        {
            CleanOutPutSlot();
        }
    }

    private void GetItemForOutput(ItemType itemType, int amount)
    {
        foreach (Item item in _craftableItems)
        {
            if (item.ItemType == itemType)
            {
                outputSlot.item = item;
                outputSlot.Count = amount;
            }
        }
    }

    void CleanOutPutSlot()
    {
        outputSlot.Count = 0;
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

        CleanOutPutSlot();
    }
}
