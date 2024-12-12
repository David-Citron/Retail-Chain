using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    public static List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    void Start()
    {
        recipes.Add(new CraftingRecipe(MachineType.Printer, new List<ItemType>() { ItemType.EmptyBooks }, ItemType.Books, 8));
    }

    void Update() {}

    public static List<CraftingRecipe> GetRecipesFor(ItemType itemType)
    {
        return recipes.FindAll(recipe => recipe.inputs.Contains(itemType));
    }

    public static CraftingRecipe FindRecipe(MachineType machineType, List<ItemType> itemTypes)
    {
        return recipes.Find(recipe => recipe.machineType == machineType && recipe.inputs.Equals(itemTypes));
    }
}
