using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Machine : MonoBehaviour, IMachine
{
    protected List<CraftingRecipe> possibleRecipes = new List<CraftingRecipe>();
    protected List<GameObject> currentItems = new List<GameObject>();
    protected GameObject resultItem = null;

    protected MachineType machineType;
    protected MachineState machineState = MachineState.Idling;

    private ActionTimer actionTimer;
    protected CraftingRecipe currentRecipe;

    [SerializeField] private GameObject resultPlace;
    [SerializeField] private GameObject[] inputPlaces;
    [SerializeField] private Animator animator;

    protected bool isWithinTheRange = false;

    public Machine(MachineType machineType)
    {
        this.machineType = machineType;
    }

    protected void Start()
    {
        foreach (var recipe in CraftingManager.recipes)
        {
            if (recipe.machineType != machineType) continue;
            possibleRecipes.Add(recipe);
        }
    }

    protected void Update()
    {
        if (!isWithinTheRange) return;

        PickUp(); // Check if the player wants to pickup the input or the result.

        if (!Input.GetKeyDown(KeyCode.Space)) return;


        if(machineState != MachineState.Ready && currentItems.Count < inputPlaces.Length)
        {
            PlayerPickUp.Instance().IfPresent(handler =>
            {
                var item = handler.holdingItem;
                if (item == null)  return;
                PutItem(item, handler);
            });
            return;
        }

        if (currentRecipe == null || actionTimer != null || machineState != MachineState.Ready || CooldownHandler.IsUnderCreateIfNot(machineType + "_working", 1)) return;

        ChangeMachineState(MachineState.Working);
        actionTimer = new ActionTimer(() => Input.GetKey(KeyCode.Space), () => {
            ChangeMachineState(MachineState.Done);
        }, () => {
            actionTimer = null;
            ChangeMachineState(MachineState.Ready);
        }, currentRecipe.time, 1).Run();
    }

    protected virtual void PickUp()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        
        if (machineState == MachineState.Done && resultItem != null)
        {
            ChangeMachineState(MachineState.Idling);
            PlayerPickUp.Instance().IfPresent(handler => handler.PickUp(resultItem));
            resultItem = null;
            return;
        }

        if (machineState == MachineState.Working || currentItems.Count <= 0 || CooldownHandler.IsUnderCreateIfNot(machineType + "_pickUp", 1)) return;
        var item = currentItems[currentItems.Count - 1];
        PlayerPickUp.Instance().IfPresent(handler => handler.PickUp(item));
        currentItems.Remove(item);
        ChangeMachineState(MachineState.Idling);
    }

    protected virtual void PutItem(GameObject input, PlayerPickUp handler)
    {
        ItemType inputType = Item.GetHoldingType(input).GetValueOrDefault();
        if(!IsValid(inputType))
        {
            Debug.LogError("invalid item");
            //Notify player that this input has no matching recipe for this machine.
            return;
        }

        if (CooldownHandler.IsUnderCreateIfNot(machineType + "_putItem", 1)) return;

        handler.DropHoldingItem();
        currentItems.Add(input);
        PlaceItem(input, true);
        
        var craftingRecipe = CraftingManager.FindRecipe(GetMachineType(), GetCurrentItems());
        if(craftingRecipe == null)
        {
            Debug.LogError("no recipe");
            //Notify player that this input has no matching recipe -- requires another item.
            return;
        }

        Debug.LogError("ready");
        //Inform that its ready.
        currentRecipe = craftingRecipe;
        ChangeMachineState(MachineState.Ready);
    }

    protected void ChangeMachineState(MachineState newState)
    {
        if (machineState == newState) return;
        machineState = newState;

        if (machineState != MachineState.Working)
        {
            PlayerMovement.freeze = false;
            PlayerPickUp.PlayerAnimator().IfPresent(animator => animator.SetBool("working", false));
        }

        switch (machineState)
        {
            case MachineState.Ready:
                if (currentItems.Count <= 0) ChangeMachineState(MachineState.Idling); // Check if something messed up and there is no items => change state back to Idling
                //Show player that he can press "space" to create the result
                break;

            case MachineState.Working:
                PlayerPickUp.PlayerAnimator().IfPresent(animator => animator.SetBool("working", true));
                PlayerMovement.freeze = true;
                //Start animation
                break;

            case MachineState.Done:
                resultItem = GetGameObjectFromPrefab(currentRecipe.output);
                PlaceItem(resultItem, false);
                GetCurrentGameObjects().ForEach(item => Destroy(item));

                currentRecipe = null;
                actionTimer = null;
                currentItems.Clear();

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
    protected GameObject GetGameObjectFromPrefab(ItemType type)
    {
        Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Items/" + type + ".prefab", typeof(GameObject));
        return (GameObject) Instantiate(prefab);
    }
    

    protected bool IsValid(ItemType input)
    {
        if (possibleRecipes.Count == 0)
        {
            Debug.LogError("no recipes");
            return false;
        }

        foreach (var recipe in possibleRecipes)
        {
            if (recipe.inputs.Contains(input)) continue;
            Debug.LogError("here 2");
            return false;
        }

        if (GetCurrentItems().Count != 0)
        {
            bool result = false;
            var copy = GetCurrentItems();
            copy.Add(input);

            foreach (var recipe in possibleRecipes)
            {
                if (!CraftingManager.ContainsAllItems(recipe, copy)) continue;
                result = true;
            }

            Debug.LogError("here 1");
            return result;
        }

        return true;
    }

    protected void PlaceItem(GameObject item, bool isInput)
    {
        GameObject place = isInput ? inputPlaces[currentItems.IndexOf(item)] : resultPlace;
        item.transform.SetParent(place.transform);

        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        if (rigidbody == null) return;
        rigidbody.isKinematic = true;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isWithinTheRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isWithinTheRange = false;
    }

    protected List<ItemType> GetCurrentItems()
    {
        List<ItemType> list = new List<ItemType>();
        GetCurrentGameObjects().ForEach(item => Item.GetHoldingType(item).IfPresent(itemType => list.Add(itemType)));
        return list;
    }

    public List<GameObject> GetCurrentGameObjects() => currentItems;
    public MachineType GetMachineType() => machineType;
    public GameObject GetResultPlace() => resultPlace;
    public MachineState GetMachineState() => machineState;
    public GameObject[] GetInputPlaces() => inputPlaces;
    
}

public enum MachineState
{
    Idling, //No items inserted or just one of them
    Ready, //Machine is ready to start the process
    Working, //Working state
    Done, //When the item is done - ready to pick up
}