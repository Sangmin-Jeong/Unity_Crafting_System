using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecipeType
{
    PLANK,
    STICK,
    TORCH,
    WOODEN_PICKAXE,
    WOODEN_SWORD,
    COUNT,
}

public static class Plank
{
    private static RecipeType _recipeType = RecipeType.PLANK;
    public static int _requiredAmount = 1;
    public static int _recipe;

    static Plank()
    {
        _recipe = 1;
    }
}

public static class Stick
{
    private static RecipeType _recipeType = RecipeType.STICK;
    public static int _requiredAmount = 2;
    public static int[,] _recipe = new int[2,1];

    static Stick()
    {
        _recipe[0, 0] = 3;
        _recipe[1, 0] = 3;
    }
}

// public static class Plank
// {
//     private static RecipeType _recipeType = RecipeType.TORCH;
//     public static int _requiredAmount = 1;
//     public static int[,] _recipe = new int[3,3];
//
//     static Plank()
//     {
//         _recipe[0, 0] = 1; _recipe[0, 1] = 1; _recipe[0, 2] = 1;
//         _recipe[1, 0] = 1; _recipe[1, 1] = 1; _recipe[1, 2] = 1;
//         _recipe[2, 0] = 1; _recipe[2, 1] = 1; _recipe[2, 2] = 1;
//     }
// }

public static class WoodenPickaxe
{
    private static RecipeType _recipeType = RecipeType.WOODEN_PICKAXE;
    public static int _requiredAmount = 3;
    public static int[,] _recipe = new int[3,3];

    static WoodenPickaxe()
    {
        _recipe[0, 0] = 2; _recipe[0, 1] = 2; _recipe[0, 2] = 2;
        _recipe[1, 0] = 0; _recipe[1, 1] = 3; _recipe[1, 2] = 0;
        _recipe[2, 0] = 0; _recipe[2, 1] = 3; _recipe[2, 2] = 0;
    }
}

// public static class WoodenSword
// {
//     private static RecipeType _recipeType = RecipeType.WOODEN_SWORD;
//     public static int _requiredAmount = 1;
//     public static int[,] _recipe = new int[3,3];
//
//     static Plank()
//     {
//         _recipe[0, 0] = 1; _recipe[0, 1] = 1; _recipe[0, 2] = 1;
//         _recipe[1, 0] = 1; _recipe[1, 1] = 1; _recipe[1, 2] = 1;
//         _recipe[2, 0] = 1; _recipe[2, 1] = 1; _recipe[2, 2] = 1;
//     }
// }
