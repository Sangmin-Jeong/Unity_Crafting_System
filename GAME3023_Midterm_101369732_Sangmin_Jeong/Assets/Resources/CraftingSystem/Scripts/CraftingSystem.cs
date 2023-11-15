using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField][Range(1,5)] private int column;
    [SerializeField][Range(1,5)]  private int row;
    
    private ItemSlot[,] craftingItemSlotsArray;
    List<ItemSlot> craftingItemSlots = new List<ItemSlot>();
    List<ItemSlot> inventoryItemSlots = new List<ItemSlot>();
    private ItemSlot outputSlot;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private Item[] _craftableItems;
    
    void Start()
    {
        outputSlot = GameObject.Find("OutputSlot").GetComponent<ItemSlot>();
        outputSlot.OnTaken += ItemSlot_OnTaken;
        
        // Set flexible Crafting Array
        Rect rect = craftingPanel.GetComponent<RectTransform>().rect;
        rect.width = column * 64f + 50f;
        craftingPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, rect.height);
        craftingPanel.GetComponent<GridLayoutGroup>().constraintCount = column;
        
        //Create crafting array with desired size
        craftingItemSlotsArray = new ItemSlot[row, column];
        int slotCounter = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                craftingItemSlots.Add(Instantiate(
                    _itemSlotPrefab, 
                    Vector3.zero, 
                    Quaternion.identity, 
                    craftingPanel.transform).GetComponent<ItemSlot>());
                craftingItemSlots[slotCounter].OnTransferred += ItemSlot_OnTransferred;
                craftingItemSlotsArray[i, j] = craftingItemSlots[slotCounter++];
            }
        }
        
        //Read all inventoryPanel as children of inventory panel
        inventoryItemSlots = new List<ItemSlot>(
            inventoryPanel.transform.GetComponentsInChildren<ItemSlot>()
        );
        
        
        // Subscribe slot's OnTransferred Event
        foreach (ItemSlot iSlot in inventoryItemSlots)
        {
            iSlot.OnTransferred += ItemSlot_OnTransferred;
        }
        
        // Load all Craftable Item scriptable objects
        _craftableItems = Resources.LoadAll<Item>("CraftingSystem/Items/Craftable_Ingredients");
    }
    
    private void ItemSlot_OnTransferred(object sender, EventArgs e)
    {
        CheckRecipes();
    }
    
    private void ItemSlot_OnTaken(object sender, EventArgs e)
    {
        ConsumeCraftSlots();
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
        
        // Plank Recipe
        // Only need to check Amount of wood on Crafting slots
        if (woodCount == Plank._requiredAmount && emptyCount == craftingItemSlots.Count - Plank._requiredAmount)
        {
            GetItemForOutput(ItemType.PLANK, 4);
        }
        // Stick Recipe
        else if (plankCount == Stick._requiredAmount && emptyCount == craftingItemSlots.Count - Stick._requiredAmount)
        {
            CheckAndOutput(Stick._recipe, ItemType.STICK, 4);
        }
        // Torch Recipe
        else if (coalCount == Torch._requiredAmount && stickCount == Torch._requiredAmount2 &&
                 emptyCount == craftingItemSlots.Count - (Torch._requiredAmount + Torch._requiredAmount2))
        {
            CheckAndOutput(Torch._recipe, ItemType.TORCH, 4);
        }
        // Wooden_Pickaxe Recipe
        else if (plankCount == WoodenPickaxe._requiredAmount && stickCount == WoodenPickaxe._requiredAmount2 &&
                 emptyCount == craftingItemSlots.Count - (WoodenPickaxe._requiredAmount + WoodenPickaxe._requiredAmount2))
        {
            CheckAndOutput(WoodenPickaxe._recipe, ItemType.WOODEN_PICKAXE, 1);
        }
        // Wooden_Sword Recipe
        else if (plankCount == WoodenSword._requiredAmount && stickCount == WoodenSword._requiredAmount2 &&
                 emptyCount == craftingItemSlots.Count - (WoodenSword._requiredAmount + WoodenSword._requiredAmount2))
        {
            CheckAndOutput(WoodenSword._recipe, ItemType.WOODEN_SWORD, 1);
        }
        // Wooden_Medal Recipe
        else if (stickCount == WoodenMedal._requiredAmount && plankCount == WoodenMedal._requiredAmount2 &&
                 emptyCount == craftingItemSlots.Count - (WoodenMedal._requiredAmount + WoodenMedal._requiredAmount2))
        {
            CheckAndOutput(WoodenMedal._recipe, ItemType.WOODEN_MEDAL, 1);
        }
        // Wooden_Helm Recipe
        else if (plankCount == WoodenHelm._requiredAmount &&
                 emptyCount == craftingItemSlots.Count - WoodenHelm._requiredAmount)
        {
            CheckAndOutput(WoodenHelm._recipe, ItemType.WOODEN_HELM, 1);
        }
        // Wooden_Key Recipe
        else if (stickCount == WoodenKey._requiredAmount && 
                 emptyCount == craftingItemSlots.Count - WoodenKey._requiredAmount)
        {
            CheckAndOutput(WoodenKey._recipe, ItemType.WOODEN_KEY, 1);
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
                // No need to check this condition if there is no space on the crafting array
                // to check recipe's array size 
                if (i + recipeRow > row || j + recipeCol > column)
                {
                    CleanOutPutSlot();
                    continue;
                }
                     
                ItemSlot[,] temp = CreateNewCheckingArray(recipe, i, j);
                
                // Exit and Display Output if we has found valid recipe
                if (CheckPermutation(temp, recipe, itemType, amount))
                {
                    return;
                }
            }
        }
    }

    private bool CheckPermutation(ItemSlot[,] craftingArray, int[,] recipe, ItemType itemType, int amount)
    {
        // craftingArray = new array made with the same size of recipe
        int craftingRow = craftingArray.GetLength(0);
        int craftingCol = craftingArray.GetLength(1);
        
        // Check the elements in the same position of both array.
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
        
        // If Above For-statement did not return false, All elements are same with recipe we want to make.
        GetItemForOutput(itemType, amount);
        return true;
    }

    private ItemSlot[,] CreateNewCheckingArray(int[,] recipe, int x, int y)
    {
        // Create new array that corresponding input recipe, the same size of array.
        // So new array could be 2x2, 3x3 whatever
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

    void CleanCraftSlots()
    {
        foreach (ItemSlot itemSlot in craftingItemSlotsArray)
        {
            itemSlot.Count = 0;
        }

        CleanOutPutSlot();
    }
    
    void ConsumeCraftSlots()
    {
        foreach (ItemSlot itemSlot in craftingItemSlotsArray)
        {
            itemSlot.Count -= 1;
        }

        CleanOutPutSlot();
        CheckRecipes();
    }
}
