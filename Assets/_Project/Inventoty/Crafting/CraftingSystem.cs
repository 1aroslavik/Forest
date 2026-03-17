using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance;

    public List<CraftRecipe> recipes;

    void Awake()
    {
        Instance = this;
    }

    public CraftRecipe currentRecipe;

    public void CheckRecipes(List<ItemData> items)
    {
        currentRecipe = null;

        Debug.Log("Items in craft area:");
        foreach (var i in items)
            Debug.Log(" - " + i.name);

        foreach (var recipe in recipes)
        {
            Debug.Log("Checking recipe: " + recipe.name);

            if (MatchRecipe(recipe, items))
            {
                Debug.Log("Recipe found!");

                currentRecipe = recipe;

                CraftUI.Instance.ShowResult(recipe.result);
                CraftUI.Instance.ShowCraftButton();

                return;
            }
        }

        Debug.Log("No recipe found");

        CraftUI.Instance.HideResult();
        CraftUI.Instance.HideCraftButton();
    }

    bool MatchRecipe(CraftRecipe recipe, List<ItemData> items)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            bool found = false;

            foreach (var item in items)
            {
                Debug.Log("Craft item: " + item.name);
                Debug.Log("Recipe item: " + ingredient.item.name);

                if (item == ingredient.item)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;
        }

        return true;
    }

    public void Craft()
    {
        if (currentRecipe == null) return;

        var inventory = FindObjectOfType<InventoryModel>();

        inventory.TryAdd(currentRecipe.result, currentRecipe.resultAmount);

        CraftArea.Instance.Clear();

        FindObjectOfType<InventoryView>().Render();

        // скрываем UI крафта
        CraftUI.Instance.HideResult();
        CraftUI.Instance.HideCraftButton();
        Debug.Log("CRAFT PRESSED");
        currentRecipe = null;
    }
}