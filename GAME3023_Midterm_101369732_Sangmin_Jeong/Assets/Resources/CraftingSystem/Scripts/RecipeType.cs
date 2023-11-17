using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// [Serializable]
// public enum RecipeType
// {
//     PLANK,
//     STICK,
//     TORCH,
//     WOODEN_PICKAXE,    
//     WOODEN_SWORD,
//     WOODEN_MEDAL,
//     WOODEN_HELM,
//     WOODEN_KEY,
//     COUNT,
// }

public static class Plank
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[1, 1];

    static Plank()
    {
        _recipeName = "Plank";
        _itemType = ItemType.PLANK;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}

public static class Stick
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[2, 1];
    
    static Stick()
    {
        _recipeName = "Stick";
        _itemType = ItemType.STICK;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}

public static class Torch
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[2, 1];
    
    static Torch()
    {
        _recipeName = "Torch";
        _itemType = ItemType.TORCH;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}

public static class WoodenPickaxe
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[3, 3];
    
    static WoodenPickaxe()
    {
        _recipeName = "WoodenPickaxe";
        _itemType = ItemType.WOODENPICKAXE;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}

public static class WoodenSword
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[3, 1];
    
    static WoodenSword()
    {
        _recipeName = "WoodenSword";
        _itemType = ItemType.WOODENSWORD;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}

public static class WoodenMedal
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[3, 1];
    
    static WoodenMedal()
    {
        _recipeName = "WoodenMedal";
        _itemType = ItemType.WOODENMEDAL;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}

public static class WoodenHelm
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[3, 3];
    
    static WoodenHelm()
    {
        _recipeName = "WoodenHelm";
        _itemType = ItemType.WOODENHELM;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}

public static class WoodenKey
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[2,2]
    {
        {(int)ItemType.STICK, (int)ItemType.STICK},
        {(int)ItemType.STICK, (int)ItemType.STICK}
    };
    
    static WoodenKey()
    {
        _recipeName = "WoodenKey";
        _itemType = ItemType.WOODENKEY;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}
public static class LoadRecipe
{
    public static void LoadRecipeFromTxt(string recipeName, int[,] recipe)
    {
        if (File.Exists("Assets/Resources/CraftingSystem/Recipes/" + recipeName  + ".txt"))
        {
            StreamReader sr = new StreamReader("Assets/Resources/CraftingSystem/Recipes/" + recipeName  + ".txt");
            
            // Check and Create new recipe array with TXT file
            int row = 0;
            int column = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                row++;
                string[] lines = line.Split(',');
                column = lines.Length;
            }
            
            // Set values from TXT file on the array just made
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < row; i++)
            {
                string line = sr.ReadLine();
                string[] lines = line.Split(',');
                for (int j = 0; j < column; j++)
                {
                    int parsedInt;
                    if (int.TryParse(lines[j], out parsedInt))
                    {
                        recipe[i, j] = parsedInt;
                    }
                }
            }
            sr.Close();
        }
        else
        {
            Debug.Log("No Recipe");
        }
    }
    
    public static void SetRequiredAmount(int[,] recipe, int[] requiredArray)
    {
        foreach (int item in recipe)
        {
            requiredArray[item]++;
            //Debug.Log($"{(ItemType)item} : " + requiredArray[item]);
        }
        
    }
}

// New Recipe class Line
//-
public static class HeartNecklaces
{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[3,3];

    static HeartNecklaces()
    {
        _recipeName = "HeartNecklaces";
        _itemType = ItemType.HEARTNECKLACES;
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }
}
