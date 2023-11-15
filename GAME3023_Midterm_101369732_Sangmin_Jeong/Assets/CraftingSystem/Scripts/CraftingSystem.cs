using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

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
        int woodCount = 0; int coalCount = 0; int stickCount = 0;
        int plankCount = 0; int emptyCount = 0;
        
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
        
        // Debug.Log("Wood: "+woodCount);
        // Debug.Log("Coal: "+coalCount);
        // Debug.Log("Stick: "+stickCount);
        // Debug.Log("Plank: "+plankCount);
        // Debug.Log("Empty: "+emptyCount);
        
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
            CheckAndOutput(Stick._recipe, ItemType.STICK, 4);
        }
        // Wooden_Pickaxe Recipe
        else if (plankCount == WoodenPickaxe._requiredAmount && stickCount == WoodenPickaxe._requiredAmount2)
        {
            CheckAndOutput(WoodenPickaxe._recipe, ItemType.WOODEN_PICKAXE, 1);
        }
        else
        {
            CleanOutPutSlot();
        }
    }

    private void CheckAndOutput(int[,] recipe, ItemType itemType, int amount)
    {
        int recipeRow = recipe.GetLength(0);
        int recipeCol = recipe.GetLength(1);

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (i + recipeRow > row || j + recipeCol > column)
                {
                    CleanOutPutSlot();
                    return;
                }
                     
                ItemSlot[,] temp = CreateNewCheckingArray(recipe, i, j);
                     
                if (CheckPermutation(temp, recipe, itemType, amount))
                {
                    return;
                }
            }
        }
    }

    private bool CheckPermutation(ItemSlot[,] craftingArray, int[,] recipe, ItemType itemType, int amount)
    {
        int craftingRow = craftingArray.GetLength(0);
        int craftingCol = craftingArray.GetLength(1);
        
        for (int i = 0; i < craftingRow; i++)
        {
            for (int j = 0; j < craftingCol; j++)
            {
                if (craftingArray[i, j].item.Id != recipe[i,j])
                {
                    CleanOutPutSlot();
                    return false;
                }
            }
        }
        
        GetItemForOutput(itemType, amount);
        return true;
    }

    private ItemSlot[,] CreateNewCheckingArray(int[,] recipe, int x, int y)
    {
        int Rrow = recipe.GetLength(0);
        int Rcolumn = recipe.GetLength(1);
        ItemSlot[,] newArray = new ItemSlot [Rrow, Rcolumn];
        
        int ni = 0;
        int nj = 0;
        if (x != -1 && y != -1)
        {
            for (int i = x; i < x + Rrow; i++)
            {
                nj = 0;
                for (int j = y; j < y + Rcolumn; j++)
                {
                    newArray[ni, nj] = craftingItemSlotsArray[i, j];
                    nj++;
                }
                ni++;
            }

            return newArray;
        }

        return null;
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
