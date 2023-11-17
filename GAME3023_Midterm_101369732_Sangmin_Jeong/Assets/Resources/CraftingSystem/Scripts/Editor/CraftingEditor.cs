using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Unity.VisualScripting;

[CustomEditor(typeof(CraftingManager))]
public class CraftingEditor : Editor
{
    private CraftingManager craftingManager;
    private bool isDisplayed = false;
    private string recipeName;
    private string rowS;
    private string columnS;
    private int row;
    private int column;
    private int amount;
    private string amountS;
    private int[,] recipeGrid = new int[,]{};

    private void OnEnable()
    {
        craftingManager = (CraftingManager)target;
        isDisplayed = false;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space(40);
        EditorGUILayout.LabelField("======== Crafting Manager ========", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        // Load values from PlayerPrefs
        craftingManager.craftingRow = PlayerPrefs.GetInt("CraftingRow", craftingManager.craftingRow);
        craftingManager.craftingColumn = PlayerPrefs.GetInt("CraftingColumn", craftingManager.craftingColumn);
        
        // Crafting grid size sliders
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Crafting Grid Size", EditorStyles.boldLabel);
        craftingManager.craftingRow = EditorGUILayout.IntSlider("Crafting Grid Row",craftingManager.craftingRow, 1, 5);
        craftingManager.craftingColumn = EditorGUILayout.IntSlider("Crafting Grid Column",craftingManager.craftingColumn, 1, 5);
        serializedObject.ApplyModifiedProperties();

        // Save values to PlayerPrefs
        PlayerPrefs.SetInt("CraftingRow", craftingManager.craftingRow);
        PlayerPrefs.SetInt("CraftingColumn", craftingManager.craftingColumn);
        PlayerPrefs.Save();
        
        EditorGUILayout.Space(20);

        DrawDefaultInspector();
        
        // Get a property from custom Editor script, the variable need to be [serializeField]
        SerializedProperty iconProperty = serializedObject.FindProperty("icon");
        
        if (GUILayout.Button("Select Sprite"))
        {
            // Open Select Sprite window
            EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, "", (int)iconProperty.propertyType);
        }
        
        // Check if the select window has been closed
        if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerObject() != null)
        {
            // Update the serialized property with the selected sprite
            iconProperty.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.LabelField("RecipeName", EditorStyles.boldLabel);
        recipeName = EditorGUILayout.TextField("",recipeName, EditorStyles.textField);
        
        EditorGUILayout.LabelField("Row", EditorStyles.boldLabel);
        rowS = EditorGUILayout.TextField("", rowS, EditorStyles.numberField);
        
        
        EditorGUILayout.LabelField("Column", EditorStyles.boldLabel);
        columnS = EditorGUILayout.TextField("", columnS, EditorStyles.numberField);
        
        if (GUILayout.Button("Create New Recipe Table"))
        {
            if (int.TryParse(rowS, out row)) {}
            if (int.TryParse(columnS, out column)) {}
            
            if (row > 0 && column > 0 && recipeName != "")
            {
                CreateRecipeTable(recipeName, row, column);
                isDisplayed = true;
            }
            else
            {
                Debug.Log("Need to fill out Name, Row, and Column ! !");
            }
        }
        
        GUILayout.Space(10);
        if (isDisplayed)
        {
            if (recipeGrid.GetLength(0) > 0 && recipeGrid.GetLength(1) > 0)
            {
                DisplayRecipeTable(recipeGrid);
                EditorGUILayout.LabelField("Amount of Output", EditorStyles.boldLabel);
                amountS = EditorGUILayout.TextField("", amountS, EditorStyles.numberField);
                if (int.TryParse(amountS, out amount)) {}
                
                if (GUILayout.Button("Create New Recipe"))
                {
                    if (amount <= 0)
                    {
                        Debug.Log("Need to fill out Amount ! !");
                    }
                    else
                    {
                        AddNewRecipe();
                        SetNewRecipe();
                        AddNewItem();
                        AddNewRecipeChecker();
                        CreateNewItemSO();

                        isDisplayed = false;
                    }

                }
            }
        }
        
        // Repaint the inspector
        Repaint();
    }

    void SetNewRecipe()
    {
        // Create TXT file for Recipe made from Editor script
        StreamWriter sw = new StreamWriter("Assets/Resources/CraftingSystem/Recipes/" + recipeName  + ".txt");
        
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                sw.Write(recipeGrid[i,j] + ",");
            }
            sw.Write("\n");
        }
        sw.Close();
    }
    
    void AddNewRecipe()
    {
        // To add new class recipe on the exist class script.
        
        string scriptPath = "Assets/Resources/CraftingSystem/Scripts/RecipeType.cs";
            //= EditorUtility.OpenFilePanel("RecipeType", "Assets/Resources/CraftingSystem/Scripts/", "cs");

        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Script path is NOT valid or not selected");
            return;
        }

        string scriptContent = File.ReadAllText(scriptPath);
        
        // Check if there is the same class recipe by name
        if (scriptContent.Contains($"public static class {recipeName}"))
        {
            Debug.LogWarning($"Recipe '{recipeName}' already exists in the script");
            return;
        }
        
        // Add new recipe class with string
        string newClassContent = $@"
public static class {recipeName}
{{
    public static string _recipeName;
    public static ItemType _itemType;
    public static int[] _requiredAmounts = new int[(int)ItemType.COUNT];
    public static int[,] _recipe = new int[{row},{column}];

    static {recipeName}()
    {{
        _recipeName = ""{recipeName}"";
        _itemType = ItemType.{recipeName.ToUpper()};
        LoadRecipe.LoadRecipeFromTxt(_recipeName, _recipe);
        LoadRecipe.SetRequiredAmount(_recipe, _requiredAmounts);
    }}
}}
";
        // Add new recipe string to the original string of the recipe class
        scriptContent += newClassContent;
        File.WriteAllText(scriptPath, scriptContent);
        AssetDatabase.Refresh();
        Debug.Log($"Recipe Class '{recipeName}' added to the script");
    }

    void AddNewItem()
    {
        // To Add new enum on the ItemType enum class
        string scriptPath = "Assets/Resources/CraftingSystem/Scripts/ItemType.cs";

        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Script path is NOT valid or not selected.");
            return;
        }

        string scriptContent = File.ReadAllText(scriptPath);
        
        if (scriptContent.Contains($"{recipeName.ToUpper()}"))
        {
            Debug.LogWarning($"{recipeName.ToUpper()} already exists in the script.");
            return;
        }

        string replacedContent = scriptContent.Replace(@"COUNT,
}", $@"{recipeName.ToUpper()},
    COUNT,
}}");
        
        scriptContent = replacedContent;
        File.WriteAllText(scriptPath, scriptContent);
        AssetDatabase.Refresh();
        Debug.Log($"ItemType '{recipeName}' added to the enum");
    }
    
    void AddNewRecipeChecker()
    {
        // To Add new enum on the ItemType enum class
        string scriptPath = "Assets/Resources/CraftingSystem/Scripts/CraftingSystem.cs";

        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Script path is NOT valid or not selected.");
            return;
        }

        string scriptContent = File.ReadAllText(scriptPath);
        
        string replacedContent = scriptContent.Replace("//-", $@"        
        if (CheckArraysAreSame(slotItemArray, {recipeName}._requiredAmounts))
        {{
            CheckAndOutput({recipeName}._recipe, {recipeName}._itemType, {amount});
        }}
        //-");
        scriptContent = replacedContent;
        File.WriteAllText(scriptPath, scriptContent);
        AssetDatabase.Refresh();
        Debug.Log($"Checker '{recipeName}' added to the Checking process");
    }
    
    void CreateRecipeTable(string recipeName, int row, int column)
    {
        // To Create base recipe grid.
        // recipeGrid made by this, will be used for dropdown editor UI.
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
        // To display enum dropdown UI
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
    
    void CreateNewItemSO()
    {
        // Create an instance of the Item scriptable object
        Item newItem = ScriptableObject.CreateInstance<Item>();
        
        // Set values for the new item
        newItem.icon = craftingManager.icon;
        newItem.ItemType = (ItemType)((int)ItemType.COUNT);
        newItem.Id = (int)ItemType.COUNT;
        newItem.description = "Description";
        newItem.isConsumable = false;

        // Save Scriptable object into desired folder
        string assetPath = $"Assets/Resources/CraftingSystem/Items/Craftable_Ingredients/{recipeName}.asset";
        AssetDatabase.CreateAsset(newItem, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("New item created and saved");
    }
}
