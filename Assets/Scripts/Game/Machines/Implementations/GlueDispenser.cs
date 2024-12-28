using System.Collections.Generic;
using UnityEngine;

public class GlueDispenser : Machine
{
    private const int MAX_GLUE_AMOUNT = 50; // 50L is equal to 10x Glue Canister, one barell is 25L.
    private const int GLUE_CANISTER = 5;

    public int glueAmount;

    public GlueDispenser() : base(MachineType.GlueDispenser) { }

    protected override void Update()
    {
        if (!isWithinTheRange) return;

        if (!Input.GetKeyDown(KeyCode.Space)) return;

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

    protected override void ChangeIsWithinRange()
    {
        base.ChangeIsWithinRange();
        UpdateRecipe();
    }

    protected override void ShowHints()
    {
        if (machineState != MachineState.Ready) return;
        
        var holdingItem = PlayerPickUp.Instance().GetValueOrDefault().holdingItem;
        Hint.ShowWhile(HintText.GetHintButton(HintButton.SPACE) + (holdingItem == null ? " TO GET CANISTER" : " TO FILL DISPENSER"), () => machineState == MachineState.Ready && isWithinTheRange);
    }

    protected override void ChangeMachineState(MachineState newState)
    {
        base.ChangeMachineState(newState);

        if (newState != MachineState.Done) return;
        
        if(resultItem == null)
        {
            PlayerPickUp.Instance().IfPresent(pickUp =>
            {
                var holdingItem = pickUp.holdingItem;
                if (holdingItem == null) return;

                pickUp.DropHoldingItem();
                Destroy(holdingItem);
                glueAmount += 5;
            });

            UpdateRecipe();
            return;
        }

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            pickUp.DropHoldingItem();
            glueAmount -= 5;
            pickUp.PickUp(resultItem);
        });

        UpdateRecipe();
    }

    protected override void PlaceItem(GameObject item, bool isInput)
    {
        if (item == null) return;
        item.SetActive(false);//Hide the object.
        item.transform.SetParent(transform); //Set parent the printer for the item
        item.transform.localPosition = Vector3.zero;
    }

    private void UpdateRecipe()
    {
        if (possibleRecipes.Count == 0) return;

        if (!isWithinTheRange)
        {
            currentRecipe = null;
            return;
        }

        var holdingItem = PlayerPickUp.Instance().GetValueOrDefault().holdingItem;
        var holdingType = Item.GetHoldingType(holdingItem).GetValueOrDefault();

        currentRecipe = possibleRecipes.Find(recipe => recipe.machineType == machineType && CraftingManager.ContainsAllItems(recipe, new List<ItemType>() { holdingType }));

        if (currentRecipe == null) return;

        if (holdingType == ItemType.None && glueAmount < GLUE_CANISTER) Hint.ShowWhile("NOT ENOUGH GLUE", () => isWithinTheRange);
        else if (holdingType == ItemType.GlueBarrel && glueAmount >= MAX_GLUE_AMOUNT) Hint.ShowWhile("GLUE DISPENSER IS FULL", () => isWithinTheRange);
        else ChangeMachineState(MachineState.Ready);
    }

    public override bool PlayAnimation() => true;
}
