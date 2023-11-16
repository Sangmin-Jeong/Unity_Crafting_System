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
    public static int _requiredAmount = 1;
    public static int _recipe = (int)ItemType.WOOD;
}

[Serializable]
public static class Stick
{
    public static int _requiredAmount = 2;
    public static int[,] _recipe = new int[2,1]
    {
        {(int)ItemType.PLANK},
        {(int)ItemType.PLANK}
    };
}

public static class Torch
{
    public static int _requiredAmount = 1;
    public static int _requiredAmount2 = 1;
    public static int[,] _recipe = new int[2,1]
    {
        {(int)ItemType.COAL},
        {(int)ItemType.STICK}
    };
}

public static class WoodenPickaxe
{
    public static int _requiredAmount = 3;
    public static int _requiredAmount2 = 2;
    public static int[,] _recipe = new int[3,3]
    {
        {(int)ItemType.PLANK, (int)ItemType.PLANK, (int)ItemType.PLANK},
        {(int)ItemType.EMPTY, (int)ItemType.STICK, (int)ItemType.EMPTY},
        {(int)ItemType.EMPTY, (int)ItemType.STICK, (int)ItemType.EMPTY}
    };
}

public static class WoodenSword
{
    public static int _requiredAmount = 2;
    public static int _requiredAmount2 = 1;
    public static int[,] _recipe = new int[3,1]
    {
        {(int)ItemType.PLANK},
        {(int)ItemType.PLANK},
        {(int)ItemType.STICK}
    };
}

public static class WoodenMedal
{
    public static int _requiredAmount = 2;
    public static int _requiredAmount2 = 1;
    public static int[,] _recipe = new int[3, 1]
    {
        { (int)ItemType.STICK },
        { (int)ItemType.STICK },
        { (int)ItemType.PLANK }
    };
}

public static class WoodenHelm
{
    public static int _requiredAmount = 7;
    public static int[,] _recipe = new int[3,3]
    {
        {(int)ItemType.PLANK, (int)ItemType.PLANK, (int)ItemType.PLANK},
        {(int)ItemType.PLANK, (int)ItemType.EMPTY, (int)ItemType.PLANK},
        {(int)ItemType.PLANK, (int)ItemType.EMPTY, (int)ItemType.PLANK}
    };
}

public static class WoodenKey
{
    public static int _requiredAmount = 4;
    public static int[,] _recipe = new int[2,2]
    {
        {(int)ItemType.STICK, (int)ItemType.STICK},
        {(int)ItemType.STICK, (int)ItemType.STICK}
    };
}
public static class LoadRecipe
{
    public static void LoadRecipeFromTxt(string recipeName, int[,] recipe)
    {
        if (File.Exists("Assets/Resources/CraftingSystem/Recipes/" + recipeName  + ".txt"))
        {
            StreamReader sr = new StreamReader("Assets/Resources/CraftingSystem/Recipes/" + recipeName  + ".txt");

            int row = 0;
            int column = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                row++;
                string[] lines = line.Split(',');
                column = lines.Length;
            }
            
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
                        Debug.Log(recipe[i, j]);
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
}

