using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    public static List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    void Awake()
    {
        recipes.Add(new CraftingRecipe(MachineType.Printer, new List<ItemType>() { ItemType.EmptyBook }, ItemType.Book, 8));
        recipes.Add(new CraftingRecipe(MachineType.Cutter, new List<ItemType>() { ItemType.Wood}, ItemType.Paper, 3));

        recipes.Add(new CraftingRecipe(MachineType.GlueDispenser, new List<ItemType>() { ItemType.None }, ItemType.GlueCanister, 1));
        recipes.Add(new CraftingRecipe(MachineType.GlueDispenser, new List<ItemType>() { ItemType.GlueBarrel }, ItemType.None, 1));

        recipes.Add(new CraftingRecipe(MachineType.Workbench, new List<ItemType>() { ItemType.GlueCanister, ItemType.Paper }, ItemType.EmptyBook, 3));

        foreach (var item in ItemManager.GetAllSellableItemData())
        {
            recipes.Add(new CraftingRecipe(MachineType.PackagingTable, new List<ItemType>() { ItemType.Package, item.itemType }, ItemType.Package, 1));
        }
    }

    void Start() {}
    void Update() {}

    public static bool HasRecipesInMachine(MachineType machineType, ItemType itemType) => GetRecipesFor(itemType).Any(recipe => recipe.machineType == machineType);
    public static List<CraftingRecipe> GetRecipesFor(ItemType itemType) => recipes.FindAll(recipe => recipe.inputs.Contains(itemType));

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
