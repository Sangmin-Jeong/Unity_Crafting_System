using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private int column;
    [SerializeField] private int row;
    
    [SerializeField]
    GameObject inventoryPanel;

    void Start()
    {
        //Read all itemSlots as children of inventory panel
        itemSlots = new List<ItemSlot>(
            inventoryPanel.transform.GetComponentsInChildren<ItemSlot>()
            );

        
        // Set Slot ID
        int num = 11;
        int slotCounter = 0;
        
        for (int j = 0; j < row; j++)
        {
            for (int k = 0; k < column; k++)
            {
                itemSlots[slotCounter++].Id = num + k;
            }
            num += 10;
        }
    }
}
