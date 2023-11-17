using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;
    
    [SerializeField] public Sprite icon;
    [HideInInspector]public CraftingSystem _craftingSystem;
    [HideInInspector]public Inventory _inventory;
    [HideInInspector][SerializeField] public int craftingRow;
    [HideInInspector][SerializeField] public int craftingColumn;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _craftingSystem = GameObject.Find("CraftingWindow").GetComponent<CraftingSystem>();
        _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }
}
