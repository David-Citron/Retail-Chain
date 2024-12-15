using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Machine : MonoBehaviour, IMachine
{
    protected List<GameObject> currentItems = new List<GameObject>();

    protected MachineType machineType;
    protected MachineState machineState = MachineState.Idling;

    private ActionTimer actionTimer;
    protected CraftingRecipe currentRecipe;

    [SerializeField] private GameObject resultPlace;
    [SerializeField] private GameObject[] inputPlaces;
    [SerializeField] private Animator animator;

    public Machine(MachineType machineType)
    {
        this.machineType = machineType;
    }

    void Start()
    {
    }

    protected void Update()
    {
        //if (machineState != MachineState.Ready || !Input.GetKeyDown(KeyCode.Space) || actionTimer != null || currentRecipe == null) return;
        if (!Input.GetKeyDown(KeyCode.Space) || actionTimer != null) return;

        Debug.Log("Started.");
        actionTimer = new ActionTimer(() => Input.GetKey(KeyCode.Space), () => {
            ChangeMachineState(MachineState.Done);
            Debug.Log("When done.");
        },
        () => {
            actionTimer = null;
            ChangeMachineState(MachineState.Ready);
            Debug.Log("Failed.");
        }, 10, 1).Run();

        // var output = GetPrefab(currentRecipe.output);
        // output.transform.position = resultPlace.transform.position;
    }

    protected virtual void PickUp()
    {
        if (machineState != MachineState.Done) return;
        ChangeMachineState(MachineState.Idling);
    }

    protected virtual void PutItem(GameObject input)
    {
        if(!IsValid(ValueOf(input)))
        {
            //Notify player that this input has no matching recipe for this machine.
            return;
        }

        var craftingRecipe = CraftingManager.FindRecipe(machineType, GetCurrentItems());
        if(craftingRecipe == null)
        {
            //Notify player that this input has no matching recipe.
            return;
        }

        currentRecipe = craftingRecipe;
        ChangeMachineState(MachineState.Ready);
    }

    protected void ChangeMachineState(MachineState newState)
    {
        if (machineState == newState) return;
        machineState = newState;

        switch(machineState)
        {
            case MachineState.Ready:
                //Show player that he can press "space" to create the result
                break;
            case MachineState.Working:
                //Start animation
                break;
            case MachineState.Done:
                currentRecipe = null;
                actionTimer = null;

                Debug.Log("Done.");
                //Stop animation, there should be also some sparkles as a finish effect?
                break;
        }
    }

    /// <summary>
    /// Gets the prefab item based on the ItemType.
    /// </summary>
    /// <param name="type">The ItemType</param>
    /// <returns>New gameobject</returns>
    protected virtual GameObject GetPrefab(ItemType type)
    {
        return Resources.Load<GameObject>("Prefabs/" + type);
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

    public List<GameObject> GetCurrentGameObjects()
    {
        return currentItems;
    }

    protected List<ItemType> GetCurrentItems()
    {
        List<ItemType> list = new List<ItemType>();

        GetCurrentGameObjects().ForEach(item => list.Add(ValueOf(item)));
        return list;
    }

    protected ItemType ValueOf(GameObject gameObject)
    {
        return (ItemType)Enum.Parse(typeof(ItemType), gameObject.tag.Replace("Item", ""));
    }

    public MachineType GetMachineType()
    {
        return machineType;
    }

    public GameObject GetResultPlace()
    {
        return resultPlace;
    }

    public MachineState GetMachineState()
    {
        return machineState;
    }

    public GameObject[] GetInputPlaces()
    {
        return inputPlaces;
    }
}

public enum MachineState
{
    Idling, //No items inserted or just one of them
    Ready, //Machine is ready to start the process
    Working, //Working state
    Done, //When the item is done - ready to pick up
}