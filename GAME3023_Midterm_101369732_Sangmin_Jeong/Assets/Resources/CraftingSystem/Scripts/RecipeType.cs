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
    WOODEN_MEDAL,
    WOODEN_HELM,
    WOODEN_KEY,
    COUNT,
}

public static class Plank
{
    public static int _requiredAmount = 1;
    public static int _recipe;

    static Plank()
    {
        _recipe = (int)ItemType.WOOD;
    }
}

public static class Stick
{
    public static int _requiredAmount = 2;
    public static int[,] _recipe = new int[2,1];

    static Stick()
    {
        _recipe[0, 0] = (int)ItemType.PLANK;
        _recipe[1, 0] = (int)ItemType.PLANK;
    }
}

public static class Torch
{
    public static int _requiredAmount = 1;
    public static int _requiredAmount2 = 1;
    public static int[,] _recipe = new int[2,1];

    static Torch()
    {
        _recipe[0, 0] = (int)ItemType.COAL;
        _recipe[1, 0] = (int)ItemType.STICK;
    }
}

public static class WoodenPickaxe
{
    public static int _requiredAmount = 3;
    public static int _requiredAmount2 = 2;
    public static int[,] _recipe = new int[3,3];

    static WoodenPickaxe()
    {
        _recipe[0, 0] = (int)ItemType.PLANK; _recipe[0, 1] = (int)ItemType.PLANK; _recipe[0, 2] = (int)ItemType.PLANK;
        _recipe[1, 0] = (int)ItemType.EMPTY; _recipe[1, 1] = (int)ItemType.STICK; _recipe[1, 2] = (int)ItemType.EMPTY;
        _recipe[2, 0] = (int)ItemType.EMPTY; _recipe[2, 1] = (int)ItemType.STICK; _recipe[2, 2] = (int)ItemType.EMPTY;
    }
}

public static class WoodenSword
{
    public static int _requiredAmount = 2;
    public static int _requiredAmount2 = 1;
    public static int[,] _recipe = new int[3,1];

    static WoodenSword()
    {
        _recipe[0, 0] = (int)ItemType.PLANK;
        _recipe[1, 0] = (int)ItemType.PLANK;
        _recipe[2, 0] = (int)ItemType.STICK;
    }
}

public static class WoodenMedal
{
    public static int _requiredAmount = 2;
    public static int _requiredAmount2 = 1;
    public static int[,] _recipe = new int[3,1];

    static WoodenMedal()
    {
        _recipe[0, 0] = (int)ItemType.STICK;
        _recipe[1, 0] = (int)ItemType.STICK;
        _recipe[2, 0] = (int)ItemType.PLANK;
    }
}

public static class WoodenHelm
{
    public static int _requiredAmount = 7;
    public static int[,] _recipe = new int[3,3];

    static WoodenHelm()
    {
        _recipe[0, 0] = (int)ItemType.PLANK; _recipe[0, 1] = (int)ItemType.PLANK; _recipe[0, 2] = (int)ItemType.PLANK;
        _recipe[1, 0] = (int)ItemType.PLANK; _recipe[1, 1] = (int)ItemType.EMPTY; _recipe[1, 2] = (int)ItemType.PLANK;
        _recipe[2, 0] = (int)ItemType.PLANK; _recipe[2, 1] = (int)ItemType.EMPTY; _recipe[2, 2] = (int)ItemType.PLANK;
    }
}

public static class WoodenKey
{
    public static int _requiredAmount = 4;
    public static int[,] _recipe = new int[2,2];

    static WoodenKey()
    {
        _recipe[0, 0] = (int)ItemType.STICK; _recipe[0, 1] = (int)ItemType.STICK;
        _recipe[1, 0] = (int)ItemType.STICK; _recipe[1, 1] = (int)ItemType.STICK;
        
    }
}
