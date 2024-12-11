using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Machine : MonoBehaviour, IMachine
{

    protected MachineType machineType;
    protected List<ItemType> currentItems = new List<ItemType>();

    [SerializeField] private GameObject resultPlace;
    [SerializeField] private GameObject[] inputPlaces;

    protected bool isReadyToPickUp;

    public Machine(MachineType machineType)
    {
        this.machineType = machineType;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected void PutItem(GameObject input)
    {
        if(!IsValid(ValueOf(input)))
        {
            //Notify player that this input has no matching recipe.
            return;
        }

        //Place item into the machine.
    }

    protected bool IsValid(ItemType input)
    {
        List<CraftingRecipe> possibleRecipes = new List<CraftingRecipe>();
        if (possibleRecipes.Count == 0) return false; //If there is not matching crafting for the input
        if (possibleRecipes.Any(recipe => recipe.machineType != GetMachineType())) return false; //If the input item isn't meant for this machine type.

        if (GetCurrentItems().Count != 0)
        {
            var copy = GetCurrentItems();
            copy.Add(input);

            foreach (var item in possibleRecipes)
            {
                if (item.inputs.Equals(copy)) continue;
                return false; //Returns false if there is no recipe for the current items + new item
            }
        }

        return true;
    }

    protected ItemType ValueOf(GameObject gameObject)
    {
        return (ItemType)Enum.Parse(typeof(ItemType), gameObject.tag.Replace("Item", ""));
    }

    public List<ItemType> GetCurrentItems()
    {
        return currentItems;
    }

    public MachineType GetMachineType()
    {
        return machineType;
    }

    public GameObject GetResultPlace()
    {
        return resultPlace;
    }

    public bool IsReadyToPickUp()
    {
        return isReadyToPickUp;
    }

    public GameObject[] GetInputPlaces()
    {
        return inputPlaces;
    }
}
