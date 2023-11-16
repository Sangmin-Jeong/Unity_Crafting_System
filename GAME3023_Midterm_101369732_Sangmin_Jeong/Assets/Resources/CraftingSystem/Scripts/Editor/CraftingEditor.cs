using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(CraftingManager))]
public class CraftingEditor : Editor
{
    private bool isDisplayed = false;
    private string recipeName;
    private string rowS;
    private string columnS;
    private int row;
    private int column;

    private int[,] recipeGrid = new int[,]{};
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Draw the default inspector
        //DrawDefaultInspector();

        CraftingManager craftingManager = (CraftingManager)target;


        EditorGUILayout.LabelField("RecipeName", EditorStyles.boldLabel);
        recipeName = EditorGUILayout.TextField("",recipeName, EditorStyles.textField);
        
        EditorGUILayout.LabelField("Row", EditorStyles.boldLabel);
        rowS = EditorGUILayout.TextField("", rowS, EditorStyles.numberField);
        
        
        EditorGUILayout.LabelField("Column", EditorStyles.boldLabel);
        columnS = EditorGUILayout.TextField("", columnS, EditorStyles.numberField);
        
        if (GUILayout.Button("Create New Recipe Table"))
        {
        
            if (int.TryParse(rowS, out row))
            if (int.TryParse(columnS, out column))
                
            if (row > 0 && column > 0)
            {
                CreateRecipeTable(recipeName, row, column);
                isDisplayed = true;
            }
        }
        
        GUILayout.Space(10);
        if (isDisplayed)
        {
            if (recipeGrid.GetLength(0) > 0 && recipeGrid.GetLength(1) > 0)
            {
                if (GUILayout.Button("Create New Recipe"))
                {
                    AddNewRecipe();
                    SetNewRecipe();
                }
                
                DisplayRecipeTable(recipeGrid);
            }
        }
        
        // Repaint the inspector
        Repaint();
    }

    void SetNewRecipe()
    {
        Type type = Type.GetType(recipeName);

        if (type != null && type.IsClass && type.IsSealed && type.IsAbstract)
        {
            Console.WriteLine($"Accessing static class: {recipeName}");
            
            FieldInfo fieldInfo = type.GetField("_recipe", BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, recipeGrid);
            }
            else
            {
                Console.WriteLine($"Method 'PrintMessage' not found in class {recipeName}.");
            }
        }
        else
        {
            Console.WriteLine($"Type {recipeName} is not a valid static class.");
        }

    }
    
    void AddNewRecipe()
    {
        string scriptPath = "Assets/Resources/CraftingSystem/Scripts/RecipeType.cs";
            //= EditorUtility.OpenFilePanel("RecipeType", "Assets/Resources/CraftingSystem/Scripts/", "cs");

        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Script path is invalid or not selected.");
            return;
        }

        string scriptContent = File.ReadAllText(scriptPath);
        
        if (scriptContent.Contains($"public static class {recipeName}"))
        {
            Debug.LogWarning($"Class '{recipeName}' already exists in the script.");
            return;
        }
        
        string newClassContent = $@"
public static class {recipeName}
{{
    // Add your new class members here
    public static int[,] _recipe = new int[{row},{column}];
}}
";
        
        scriptContent += newClassContent;
        File.WriteAllText(scriptPath, scriptContent);
        AssetDatabase.Refresh();
        Debug.Log($"Class '{recipeName}' added to the script at: {scriptPath}");
    }
    
    void CreateRecipeTable(string recipeName, int row, int column)
    {
        recipeGrid = new int[row, column];
        
        EditorGUILayout.LabelField(recipeName + " Recipe", EditorStyles.boldLabel);
        
        for (int i = 0; i < row; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < column; j++)
            {
                recipeGrid[i,j] = (int)ItemType.EMPTY;
            }
            GUILayout.EndHorizontal();
        }
    }

    void DisplayRecipeTable(int[,] recipe)
    {
        for (int i = 0; i < recipe.GetLength(0); i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < recipe.GetLength(1); j++)
            {
                int itemID = recipe[i, j];

                ItemType itemType = (ItemType)itemID;
                ItemType newItemType = (ItemType)EditorGUILayout.EnumPopup(itemType);
                recipe[i, j] = (int)newItemType;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(10);
    }
}
