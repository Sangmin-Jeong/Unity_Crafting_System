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
    public static int[,] _recipe = new int[3,3];

    static Plank()
    {
        _recipe[0, 0] = 1; _recipe[0, 1] = 1; _recipe[0, 2] = 1;
        _recipe[1, 0] = 1; _recipe[1, 1] = 1; _recipe[1, 2] = 1;
        _recipe[2, 0] = 1; _recipe[2, 1] = 1; _recipe[2, 2] = 1;
    }
}
