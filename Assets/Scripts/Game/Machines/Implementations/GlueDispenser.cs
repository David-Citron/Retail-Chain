using System.Collections.Generic;
using UnityEngine;

public class GlueDispenser : Machine
{
    private const int MAX_GLUE_AMOUNT = 50; // 50L is equal to 10x Glue Canister, one barell is 25L.
    private const int GLUE_CANISTER = 5;

    public int glueAmount;
    [SerializeField] private Renderer material;

    public GlueDispenser() : base(MachineType.GlueDispenser) { }

    protected override void OnStart()
    {
        AddInteraction(new Interaction(GetTag(), () => Input.GetKeyDown(KeyCode.Space) && isPlayerNear, i => StartInteraction(), new Hint[] {
            new Hint(HintText.GetHintButton(HintButton.SPACE) + " TO GET CANISTER", () => PlayerPickUp.holdingItem == null && machineState == MachineState.Ready),
            new Hint(HintText.GetHintButton(HintButton.SPACE) + " TO FILL GLUE DISPENSER", () => PlayerPickUp.holdingItem != null),
            new Hint("NO RECIPES FOUND", () => PlayerPickUp.GetHoldingType() != ItemType.None && PlayerPickUp.GetHoldingType() != ItemType.GlueBarrel && machineState == MachineState.Idling)
        }));
    }

    protected override void StartInteraction()
    {
        UpdateRecipe();
        if (currentRecipe == null || actionTimer != null || machineState != MachineState.Ready || CooldownHandler.IsUnderCreateIfNot(machineType + "_working", 1)) return;

        ChangeMachineState(MachineState.Working);
        if (currentRecipe.output == ItemType.None) FillDispenser();
        else CreateCanister();
    }


    private void CreateCanister()
    {
        actionTimer = new ActionTimer(() => Input.GetKey(KeyCode.Space),
            () => ChangeMachineState(MachineState.Done),
            () => {
                actionTimer = null;
                ChangeMachineState(MachineState.Ready);
            }, currentRecipe.time, 1).Run();
    }

    private void FillDispenser()
    {
        actionTimer = new ActionTimer(() => Input.GetKey(KeyCode.Space),
            () => ChangeMachineState(MachineState.Done),
            () => {
                actionTimer = null;
                ChangeMachineState(MachineState.Ready);
            }, currentRecipe.time, 1).Run();
    }

    protected override void ChangeMachineState(MachineState newState)
    {
        base.ChangeMachineState(newState);

        if (newState != MachineState.Done) return;
        
        if(resultItem == null)
        {
            PlayerPickUp.Instance().IfPresent(pickUp =>
            {
                var holdingItem = PlayerPickUp.holdingItem;
                if (holdingItem == null) return;

                pickUp.DropHoldingItem();
                Destroy(holdingItem);
                UpdateGlueAmount(5);
            });

            UpdateRecipe();
            return;
        }

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            pickUp.DropHoldingItem();
            UpdateGlueAmount(-5);
            pickUp.PickUp(resultItem);
        });

        UpdateRecipe();
    }

    protected override void PlaceItem(GameObject place, GameObject item)
    {
        if (item == null) return ;
        item.SetActive(false);//Hide the object.
        item.transform.SetParent(transform); //Set parent the printer for the item
        item.transform.localPosition = Vector3.zero;
    }

    private void UpdateRecipe()
    {
        if (possibleRecipes.Count == 0) return;
        
        var holdingType = PlayerPickUp.GetHoldingType();

        currentRecipe = possibleRecipes.Find(recipe => recipe.machineType == machineType && CraftingManager.ContainsAllItems(recipe, new List<ItemType>() { holdingType }));

        if (currentRecipe == null) return;

        if (holdingType == ItemType.None && glueAmount < GLUE_CANISTER) Hint.Create("NOT ENOUGH GLUE", 1);
        else if (holdingType == ItemType.GlueBarrel && glueAmount >= MAX_GLUE_AMOUNT) Hint.Create("GLUE DISPENSER IS FULL", 1);
        else ChangeMachineState(MachineState.Ready);
    }

    private void UpdateGlueAmount(int amount)
    {
        glueAmount += amount;

        
    }

    public override bool PlayAnimation() => true;
    public override string GetTag() => "MachineGlueDispenser";
}
