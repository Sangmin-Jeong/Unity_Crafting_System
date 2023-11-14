using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private int column;
    [SerializeField] private int row;
    List<ItemSlot> craftingItemSlots = new List<ItemSlot>();
    List<ItemSlot> inventoryItemSlots = new List<ItemSlot>();
    [SerializeField] private GameObject inventoryPanel;
    private ItemSlot outputSlot;

    [SerializeField]
    GameObject craftingPanel;

    [SerializeField] public Dictionary<RecipeType, int[,]> _recipes = new Dictionary<RecipeType, int[,]>();
    void Start()
    {
        outputSlot = GameObject.Find("OutputSlot").GetComponent<ItemSlot>();
        
        //Read all craftingItemSlots as children of inventory panel
        craftingItemSlots = new List<ItemSlot>(
            craftingPanel.transform.GetComponentsInChildren<ItemSlot>()
        );
        craftingItemSlots.Remove(outputSlot);
        
        //Read all inventoryPanel as children of inventory panel
        inventoryItemSlots = new List<ItemSlot>(
            inventoryPanel.transform.GetComponentsInChildren<ItemSlot>()
        );

        
        
        // Set Slot ID
        int num = 11;
        int slotCounter = 0;
        
        for (int j = 0; j < row; j++)
        {
            for (int k = 0; k < column; k++)
            {
                craftingItemSlots[slotCounter].Id = num + k;
                craftingItemSlots[slotCounter++].OnTransferred += ItemSlot_OnTransferred;
            }
            num += 10;
        }
        
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
        foreach (ItemSlot itemSlot in craftingItemSlots)
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
        if (woodCount == 0 && coalCount == 0 && stickCount == 0 && plankCount == 0) return;
        
        Debug.Log("Wood: "+woodCount);
        Debug.Log("Coal: "+coalCount);
        Debug.Log("Stick: "+stickCount);
        Debug.Log("Plank: "+plankCount);
        
        
    }
}
