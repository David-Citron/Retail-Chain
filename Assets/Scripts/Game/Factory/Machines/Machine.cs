using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public abstract class Machine : Interactable, IMachine
{
    protected List<CraftingRecipe> possibleRecipes = new List<CraftingRecipe>();
    protected List<GameObject> currentItems = new List<GameObject>();

    protected GameObject resultItem = null;

    protected MachineType machineType;
    protected MachineState machineState = MachineState.Idling;

    protected ActionTimer actionTimer;
    protected CraftingRecipe currentRecipe;

    [SerializeField] private GameObject resultPlace;
    [SerializeField] private List<GameObject> inputPlaces;

    public Machine(MachineType machineType)
    {
        this.machineType = machineType;
    }

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;
          
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = GetColliderSize(collider);
    }

    protected void Start()
    {
        foreach (var recipe in CraftingManager.recipes)
        {
            if (recipe.machineType != machineType) continue;
            possibleRecipes.Add(recipe);
        }

        OnStart();
    }

    protected virtual void Update() {
        InputInfo inputInfo = GetNearestSlot();
        if (inputInfo == null || !inputInfo.isInValidDistance) return;
        UpdateHints();
    }

    protected virtual void OnStart()
    {
        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear, collider => PickUp(),
            new Hint(() => Hint.GetHintButton(ActionType.PickUpItem) + " TO PICK UP", () => !PlayerPickUp.IsHodlingItem() && machineState != MachineState.Working && GetNearestSlot().IsReadyToPickUp())
        ));

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear, collider => StartInteraction(), new Hint[] {
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO INTERACT", () => machineState == MachineState.Ready && GetNearestSlot().isInValidDistance  && !PlayerPickUp.IsHodlingItem()),
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO INSERT", () => IsValid(PlayerPickUp.holdingItem) && machineState == MachineState.Idling && GetNearestSlot().IsValid()),
            new Hint("INVALID ITEM", () => PlayerPickUp.IsHodlingItem() && !IsValid(PlayerPickUp.holdingItem) && machineState == MachineState.Idling)
        }));
    }

    protected virtual void StartInteraction()
    {
        if(machineState != MachineState.Ready && (inputPlaces.Count == 0 || currentItems.Count < inputPlaces.Count))
        {
            PutItem(PlayerPickUp.holdingItem);
            return;
        }

        if (PlayerPickUp.IsHodlingItem() || machineState != MachineState.Ready) return;

        StartTimer();
        ChangeMachineState(MachineState.Working);
    }

    protected virtual void StartTimer()
    {
        actionTimer = new ActionTimer(() => HoldingKey(ActionType.Interaction),
        () => ChangeMachineState(MachineState.Done),
        () => {
            actionTimer = null;
            ChangeMachineState(MachineState.Ready);
        }, currentRecipe.time, 1).Run();
    }

    protected virtual void PickUp()
    {
        InputInfo nearestInput = GetNearestSlot();
        if(inputPlaces.Count != 0 && resultPlace != null && (nearestInput == null || !nearestInput.IsReadyToPickUp())) return;

        if (PlayerPickUp.IsHodlingItem())
        {
            Hint.Create("DROP CURRENT ITEM", 1);
            return;
        }

        if (machineState == MachineState.Done && resultItem != null)
        {
            PlayerPickUp.Instance().IfPresent(handler => handler.PickUp(resultItem));
            resultItem = null;
            ChangeMachineState(MachineState.Idling);
            return;
        }

        if (machineState == MachineState.Working || currentItems.Count <= 0) return;

        var item = nearestInput.inputPlace.gameObject.transform.GetChild(0).gameObject;
        if (item == null) return;

        if (!currentItems.Remove(item)) return;

        PlayerPickUp.Instance().IfPresent(handler => handler.PickUp(item));
        ChangeMachineState(MachineState.Idling);
    }

    protected void PutItem(GameObject item)
    {
        if(resultItem != null)
        {
            Hint.Create("TAKE THE FINISHED PRODUCT", 2);
            return;
        }

        InputInfo input = GetNearestSlot();
        if (inputPlaces.Count != 0 && resultPlace != null && (input == null || !input.IsValid())) return;

        if(!IsValid(item)) return;

        PlayerPickUp.Instance().IfPresent(handler =>
        {
            handler.DropHoldingItem();
            currentItems.Add(item);
            PlaceItem(input != null ? input.inputPlace : null, item);

            var craftingRecipe = possibleRecipes.Find(recipe => recipe.machineType == machineType && CraftingManager.ContainsAllItems(recipe, GetCurrentItems()));
            if (craftingRecipe == null) return;

            currentRecipe = craftingRecipe;

            //Inform that its ready.
            ChangeMachineState(MachineState.Ready);
        });
    }

    protected virtual void ChangeMachineState(MachineState newState)
    {
        if (machineState == newState) return;
        machineState = newState;

        if (machineState != MachineState.Working && PlayAnimation())
        {
            PlayerPickUp.Instance().IfPresent(pikUp => pikUp.animator.SetBool("working", false));
            PlayerInputManager.isInteracting = false;
        }

        switch (machineState)
        {
            case MachineState.Ready:
                if (inputPlaces.Count != 0 && currentItems.Count <= 0) ChangeMachineState(MachineState.Idling); // Check if something messed up and there is no items => change state back to Idling
                if(PlayAnimation()) CircleTimer.Stop();
                break;

            case MachineState.Working:
                if (!PlayAnimation()) break;

                CircleTimer.Start(currentRecipe.time);
                PlayerInputManager.isInteracting = true;
                PlayerPickUp.Instance().IfPresent(pikUp => pikUp.animator.SetBool("working", true));
                break;

            case MachineState.Done:
                resultItem = ItemManager.CreateItem(currentRecipe.output);
                PlaceItem(resultPlace, resultItem);
                GetCurrentGameObjects().ForEach(item => Destroy(item));

                currentRecipe = null;
                currentItems.Clear();

                actionTimer = null;
                break;
        }
        UpdateHints();
    }
    
    protected virtual bool IsValid(GameObject item)
    {
        if (possibleRecipes.Count == 0) return false;
        Item itemInfo = ItemManager.GetItemInfo(item);
        if (itemInfo != null && itemInfo.contentType != ItemType.None) return false;

        ItemType itemType = ItemManager.GetItemType(item).GetValueOrDefault();
        bool result = false;
        if (GetCurrentItems().Count != 0)
        {
            var copy = GetCurrentItems();
            copy.Add(itemType);

            foreach (var recipe in possibleRecipes)
            {
                if (!CraftingManager.ContainsAllItems(recipe, copy)) continue;
                result = true;
            }
        } else
        {
            foreach (var recipe in possibleRecipes)
            {
                if (!recipe.inputs.Contains(itemType)) continue;
                result = true;
            }
        }

        return result;
    }

    protected virtual void PlaceItem(GameObject place, GameObject item)
    {
        if (item == null) return;

        item.transform.SetParent(place.transform);

        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        if (rigidbody == null) return;
        rigidbody.isKinematic = true;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    protected InputInfo GetNearestSlot()
    {
        if (!isPlayerNear || inputPlaces.Count == 0) return new InputInfo(); //If the inputs are 0.

        GameObject nearestSlot = inputPlaces[0];
        float nearestItemDistance = Vector3.Distance(PlayerInputManager.instance.transform.position, nearestSlot.transform.position);

        for (int i = 0; i < inputPlaces.Count; i++)
        {
            float currentItemDistance = Vector3.Distance(PlayerInputManager.instance.transform.position, inputPlaces[i].transform.position);
            if (currentItemDistance > nearestItemDistance) break;
            nearestItemDistance = currentItemDistance;
            nearestSlot = inputPlaces[i];
        }
            
        if (nearestSlot.transform.childCount != 0)
        {
            Hint.ShowWhile(() => "ITEM SLOT IS FULL", () => PlayerPickUp.GetHoldingType() != ItemType.None && machineState == MachineState.Idling && GetNearestSlot().IsReadyToPickUp());
        }

        return new InputInfo(nearestSlot, nearestItemDistance <= 1.3f, nearestSlot.transform.childCount != 0);
    }

    protected List<ItemType> GetCurrentItems()
    {
        List<ItemType> list = new List<ItemType>();
        GetCurrentGameObjects().ForEach(item => ItemManager.GetItemType(item).IfPresent(itemType => list.Add(itemType)));
        return list;
    }

    public List<GameObject> GetInputPlaces() => inputPlaces;
    public List<GameObject> GetCurrentGameObjects() => currentItems;
    public GameObject GetResultPlace() => resultPlace;
    public MachineType GetMachineType() => machineType;
    public MachineState GetMachineState() => machineState;
    public virtual Vector3 GetColliderSize(BoxCollider collider) => new Vector3(collider.size.x, 1.5f, collider.size.z);
    public abstract bool PlayAnimation();
}

public enum MachineState
{
    Idling, //No items inserted or just one of them
    Ready, //Machine is ready to start the process
    Working, //Working state
    Done, //When the item is done - ready to pick up
}

public class InputInfo
{
    public GameObject inputPlace { get; private set; }
    public bool isInValidDistance { get; private set; }
    public bool isFull { get; private set; }

    public InputInfo(GameObject inputPlace, bool isInValidDistance, bool isFull)
    {
        this.inputPlace = inputPlace;
        this.isInValidDistance = isInValidDistance;
        this.isFull = isFull;
    }

    public InputInfo() : this(null, false, true) { }

    public bool IsValid() => isInValidDistance && !isFull;
    public bool IsReadyToPickUp() => isInValidDistance && isFull;
}