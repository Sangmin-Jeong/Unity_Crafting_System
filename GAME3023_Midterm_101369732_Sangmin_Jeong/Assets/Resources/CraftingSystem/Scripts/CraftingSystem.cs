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
    [SerializeField][Range(1,5)] public int column;
    [SerializeField][Range(1,5)] public int row;
    
    public ItemSlot[,] craftingItemSlotsArray;
    List<ItemSlot> craftingItemSlots = new List<ItemSlot>();
    List<ItemSlot> inventoryItemSlots = new List<ItemSlot>();
    private ItemSlot outputSlot;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private Item[] _craftableItems;

    private bool isValidRecipe = false;
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
    
    bool CheckArraysAreSame(int[] slotItems, int[] requiredItems)
    {
        // No need to Check Arrays' size are different 
        if (slotItems.Length != requiredItems.Length)
        {
            return false;
        }
        
        for (int i = 1; i < slotItems.Length - 1; i++)
        {
            // No requirement and No useless Item for recipe
            if (slotItems[i] == 0 && requiredItems[i] == 0)
            {
                continue;
            }
            // There is useless Item on the crafting slot
            if (slotItems[i] > requiredItems[i])
            {
                return false;
            }
            // Ingredients not enough on the crafting slot
            if (slotItems[i] < requiredItems[i] && requiredItems[i] > 0)
            {
                return false;
            }
        }
        // All requirements are valid 
        return true;
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
                    isValidRecipe = true;
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
    
        private void CheckRecipes()
    {   
        // Create an array to check what ingredient we have and how many.
        int[] slotItemArray = new int[(int)ItemType.COUNT];

        foreach (ItemSlot itemSlot in craftingItemSlotsArray)
        {
            slotItemArray[(int)itemSlot.item.ItemType]++;
        }
        
        isValidRecipe = false;
        
        // Check if we have enough ingredients for each recipe
        if (CheckArraysAreSame(slotItemArray, Plank._requiredAmounts))
        {
            // Check if the ingredients are located correctly compare to recipe
            CheckAndOutput(Plank._recipe, Plank._itemType, 4);
        }
        
        if (CheckArraysAreSame(slotItemArray, Stick._requiredAmounts))
        {
            CheckAndOutput(Stick._recipe, Stick._itemType, 4);
        }
        
        if (CheckArraysAreSame(slotItemArray, Torch._requiredAmounts))
        {
            CheckAndOutput(Torch._recipe, Torch._itemType, 4);
        }
        
        if (CheckArraysAreSame(slotItemArray, WoodenPickaxe._requiredAmounts))
        {
            CheckAndOutput(WoodenPickaxe._recipe, WoodenPickaxe._itemType, 1);
        }
        
        if (CheckArraysAreSame(slotItemArray, WoodenSword._requiredAmounts))
        {
            CheckAndOutput(WoodenSword._recipe, WoodenSword._itemType, 1);
        }
        
        if (CheckArraysAreSame(slotItemArray, WoodenMedal._requiredAmounts))
        {
            CheckAndOutput(WoodenMedal._recipe, WoodenMedal._itemType, 1);
        }
        
        if (CheckArraysAreSame(slotItemArray, WoodenHelm._requiredAmounts))
        {
            CheckAndOutput(WoodenHelm._recipe, WoodenHelm._itemType, 1);
        }
        
        if (CheckArraysAreSame(slotItemArray, WoodenKey._requiredAmounts))
        {
            CheckAndOutput(WoodenKey._recipe, WoodenKey._itemType, 1);
        }
        
        // New Checker Line
        //-
        
         
        if(!isValidRecipe)
        {
            CleanOutPutSlot();
        }
    }
}
