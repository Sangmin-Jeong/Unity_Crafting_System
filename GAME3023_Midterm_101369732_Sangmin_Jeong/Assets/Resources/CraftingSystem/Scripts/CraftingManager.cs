using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] public Sprite icon;
    [HideInInspector]public CraftingSystem _craftingSystem;
    [HideInInspector]public Inventory _inventory;
    
    void Start()
    {
        _craftingSystem = GameObject.Find("CraftingWindow").GetComponent<CraftingSystem>();
        _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }
}
