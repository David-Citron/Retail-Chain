using System.Collections.Generic;
using UnityEngine;

public class GlueDispenser : Machine
{
    private const int MAX_GLUE_AMOUNT = 50; // 50L is equal to 10x Glue Canister, one barrel is 25L.
    private const int GLUE_AMOUNT_PER_BARREL = 25; // Amount of glue in barrel
    private const int GLUE_CANISTER = 5;

    public int glueAmount;
    [SerializeField] private GameObject glueLiquid;

    public GlueDispenser() : base(MachineType.GlueDispenser) { }

    protected override void OnStart()
    {
        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear, i => StartInteraction(), new Hint[] {
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO GET CANISTER", () => !PlayerPickUp.IsHodlingItem() && glueAmount >= GLUE_CANISTER),
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO FILL GLUE DISPENSER", () => PlayerPickUp.GetHoldingType() == ItemType.GlueBarrel),
            new Hint("INVALID ITEM", () => PlayerPickUp.GetHoldingType() != ItemType.GlueBarrel && machineState == MachineState.Idling)
        }));
        UpdateGlueLiquid();
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
        actionTimer = new ActionTimer(() => HoldingKey(ActionType.Interaction),
            () => ChangeMachineState(MachineState.Done),
            () => {
                actionTimer = null;
                ChangeMachineState(MachineState.Ready);
            }, currentRecipe.time, 1).Run();
    }

    private void FillDispenser()
    {
        actionTimer = new ActionTimer(() => HoldingKey(ActionType.Interaction),
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
                ChangeGlueAmount(GLUE_AMOUNT_PER_BARREL);
            });

            UpdateRecipe();
            return;
        }

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            pickUp.DropHoldingItem();
            ChangeGlueAmount(-GLUE_CANISTER);
            pickUp.PickUp(resultItem);
        });

        UpdateRecipe();
    }

    protected override void PlaceItem(GameObject place, GameObject item)
    {
        if (item == null) return;
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

    private void ChangeGlueAmount(int amount)
    {
        glueAmount = Mathf.Clamp(glueAmount + amount, 0, MAX_GLUE_AMOUNT);
        UpdateGlueLiquid();
    }

    private void UpdateGlueLiquid() => glueLiquid.transform.localScale = new Vector3(1, 1, (glueAmount / (float) MAX_GLUE_AMOUNT));

    public override bool PlayAnimation() => true;
    public override string GetTag() => "MachineGlueDispenser";
}
