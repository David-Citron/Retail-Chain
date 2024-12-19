using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    public static List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    void Awake()
    {
        recipes.Add(new CraftingRecipe(MachineType.Printer, new List<ItemType>() { ItemType.EmptyBooks }, ItemType.Books, 8));
        recipes.Add(new CraftingRecipe(MachineType.Cutter, new List<ItemType>() { ItemType.Wood}, ItemType.Paper, 2));
    }

    void Start() {}

    void Update() {}

    public static List<CraftingRecipe> GetRecipesFor(ItemType itemType)
    {
        return recipes.FindAll(recipe => recipe.inputs.Contains(itemType));
    }

    public static CraftingRecipe FindRecipe(MachineType machineType, List<ItemType> itemTypes)
    {
        return recipes.Find(recipe => recipe.machineType == machineType && ContainsAllItems(recipe, itemTypes));
    }

    public static bool ContainsAllItems(CraftingRecipe recipe, List<ItemType> itemTypes)
    {
        bool result = true;
        foreach (var item in recipe.inputs)
        {
            if (itemTypes.Contains(item)) continue;
            result = false;
        }

        return result;
    }
}
