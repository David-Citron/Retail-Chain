using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Machine : MonoBehaviour, IMachine
{
    protected List<CraftingRecipe> possibleRecipes = new List<CraftingRecipe>();
    protected List<GameObject> currentItems = new List<GameObject>();
    protected GameObject resultItem = null;

    protected MachineType machineType;
    protected MachineState machineState = MachineState.Idling;

    protected ActionTimer actionTimer;
    protected CraftingRecipe currentRecipe;

    [SerializeField] private GameObject resultPlace;
    [SerializeField] private GameObject[] inputPlaces;
    [SerializeField] private Animator animator;

    public static bool isWithinTheRange = false;

    public Machine(MachineType machineType)
    {
        this.machineType = machineType;
    }

    protected virtual void Start()
    {
        foreach (var recipe in CraftingManager.recipes)
        {
            if (recipe.machineType != machineType) continue;
            possibleRecipes.Add(recipe);
        }
    }

    protected virtual void Update()
    {
        if (!isWithinTheRange) return;

        PickUp(); // Check if the player wants to pickup the input or the result.

        if (!Input.GetKeyDown(KeyCode.Space)) return;
        
        if(machineState != MachineState.Ready && (inputPlaces.Length == 0 || currentItems.Count < inputPlaces.Length))
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

        StartTimer();
        ChangeMachineState(MachineState.Working);
    }

    protected virtual void StartTimer()
    {
        actionTimer = new ActionTimer(() => Input.GetKey(KeyCode.Space),
        () => ChangeMachineState(MachineState.Done),
        () => {
            actionTimer = null;
            ChangeMachineState(MachineState.Ready);
    }, currentRecipe.time, 1).Run();
    }

    protected virtual void PickUp()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        
        if (machineState == MachineState.Done && resultItem != null)
        {
            PlayerPickUp.Instance().IfPresent(handler => handler.PickUp(resultItem));
            resultItem = null;
            ChangeMachineState(MachineState.Idling);
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
        if (CooldownHandler.IsUnderCreateIfNot(machineType + "_putItem", 1)) return;

        ItemType inputType = Item.GetHoldingType(input).GetValueOrDefault();
        if(!IsValid(inputType))  return;

        handler.DropHoldingItem();
        currentItems.Add(input);
        PlaceItem(input, true);

        var craftingRecipe = possibleRecipes.Find(recipe => recipe.machineType == machineType && CraftingManager.ContainsAllItems(recipe, GetCurrentItems()));
        if(craftingRecipe == null)  return;

        currentRecipe = craftingRecipe;

        //Inform that its ready.
        ChangeMachineState(MachineState.Ready);
    }

    protected virtual void ChangeMachineState(MachineState newState)
    {
        if (machineState == newState) return;
        machineState = newState;

        if (machineState != MachineState.Working && PlayAnimation())
        {
            PlayerMovement.freeze = false;
            PlayerPickUp.PlayerAnimator().IfPresent(animator => animator.SetBool("working", false));
        }

        switch (machineState)
        {
            case MachineState.Ready:
                if (currentItems.Count <= 0) ChangeMachineState(MachineState.Idling); // Check if something messed up and there is no items => change state back to Idling
                CircleTimer.Stop();
                break;

            case MachineState.Working:
                if(PlayAnimation())
                {
                    PlayerPickUp.PlayerAnimator().IfPresent(animator => animator.SetBool("working", true));
                    PlayerMovement.freeze = true;
                }

                CircleTimer.Start(currentRecipe.time);
                //Start animation
                break;

            case MachineState.Done:
                resultItem = GetGameObjectFromPrefab(currentRecipe.output);
                PlaceItem(resultItem, false);
                GetCurrentGameObjects().ForEach(item => Destroy(item));

                currentRecipe = null;
                actionTimer = null;
                currentItems.Clear();

                //Stop animation, there should be also some sparkles as a finish effect?
                break;
        }

        ShowHints();
    }

    /// <summary>
    /// Gets the prefab item based on the ItemType.
    /// </summary>
    /// <param name="type">The ItemType</param>
    /// <returns>New gameobject</returns>
    protected GameObject GetGameObjectFromPrefab(ItemType type)
    {
        if (type == ItemType.None) return null;

        Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Items/" + type + ".prefab", typeof(GameObject));
        if (prefab == null) return null;
        return (GameObject) Instantiate(prefab);
    }
    
    protected bool IsValid(ItemType input)
    {
        if (possibleRecipes.Count == 0)  return false;
        
        foreach (var recipe in possibleRecipes)
        {
            if (recipe.inputs.Contains(input)) continue;
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
            return result;
        }

        return true;
    }

    protected virtual void PlaceItem(GameObject item, bool isInput)
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
        ShowHints();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isWithinTheRange = false;
        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            if (pickUp.holdingItem == null) return;
            Hint.ShowWhile(HintText.GetHintButton(HintButton.Q) + " TO DROP", () => pickUp.holdingItem != null);
        });
    }

    protected List<ItemType> GetCurrentItems()
    {
        List<ItemType> list = new List<ItemType>();
        GetCurrentGameObjects().ForEach(item => Item.GetHoldingType(item).IfPresent(itemType => list.Add(itemType)));
        return list;
    }

    protected virtual void ShowHints()
    {
        if (machineState == MachineState.Ready)
        {
            Hint.ShowWhile(HintText.GetHintButton(HintButton.SPACE) + " TO INTERACT", () => machineState == MachineState.Ready && isWithinTheRange);
            Hint.ShowWhile(HintText.GetHintButton(HintButton.E) + " TO PICK UP", () => machineState == MachineState.Ready && isWithinTheRange);
            return;
        }

        PlayerPickUp.Instance().IfPresent(handler => {
            ItemType itemType = Item.GetHoldingType(handler.holdingItem).GetValueOrDefault();

            if (itemType == ItemType.None && machineState == MachineState.Done)
            {
                Hint.ShowWhile(HintText.GetHintButton(HintButton.E) + " TO PICK UP", () => isWithinTheRange && machineState == MachineState.Done);
                return;
            }

            if (machineState != MachineState.Idling) return;

            bool anyRecipe = CraftingManager.HasRecipesInMachine(machineType, itemType);

            if (anyRecipe) Hint.ShowWhile(HintText.GetHintButton(HintButton.SPACE) + " TO INSERT", () => itemType != ItemType.None && isWithinTheRange && anyRecipe && machineState == MachineState.Idling);
            else Hint.ShowWhile("NO RECIPES FOUND", () => itemType != ItemType.None && isWithinTheRange && !anyRecipe && machineState == MachineState.Idling);
        });
    }

    public List<GameObject> GetCurrentGameObjects() => currentItems;
    public MachineType GetMachineType() => machineType;
    public GameObject GetResultPlace() => resultPlace;
    public MachineState GetMachineState() => machineState;
    public GameObject[] GetInputPlaces() => inputPlaces;

    public abstract bool PlayAnimation();
}

public enum MachineState
{
    Idling, //No items inserted or just one of them
    Ready, //Machine is ready to start the process
    Working, //Working state
    Done, //When the item is done - ready to pick up
}