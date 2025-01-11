using UnityEngine;

public class Printer : Machine
{
    public Printer() : base(MachineType.Printer) { }

    protected override void OnStart()
    {
        AddInteraction(new Interaction(GetTag(), () => Input.GetKeyDown(KeyCode.E), collider => PickUp(),
            new Hint(HintText.GetHintButton(HintButton.E) + " TO PICK UP", () => PlayerPickUp.GetHoldingType() == ItemType.None && (machineState == MachineState.Done || machineState == MachineState.Ready))
        ));

        AddInteraction(new Interaction(GetTag(), () => Input.GetKeyDown(KeyCode.Space), collider => StartInteraction(), new Hint[] {
            new Hint(HintText.GetHintButton(HintButton.SPACE) + " TO PRINT", () => machineState == MachineState.Ready),
            new Hint(HintText.GetHintButton(HintButton.SPACE) + " TO INSERT", () => IsValid(PlayerPickUp.GetHoldingType()) && machineState == MachineState.Idling),
            new Hint("INVALID ITEM", () => !IsValid(PlayerPickUp.GetHoldingType()) && machineState == MachineState.Idling)
        }));
    }

    protected override void Update() { }

    protected override void StartTimer()
    {
        actionTimer = new ActionTimer(
            () => ChangeMachineState(MachineState.Done),
            () => {
                actionTimer = null;
                ChangeMachineState(MachineState.Ready);
            }, currentRecipe.time, 1).Run();
    }

    protected override void PlaceItem(GameObject place, GameObject item) {
        item.SetActive(false);//Hide the object.
        item.transform.SetParent(transform); //Set parent the printer for the item
        item.transform.localPosition = Vector3.zero;
    }

    protected override void ChangeMachineState(MachineState newState)
    {
        base.ChangeMachineState(newState);
        if(machineState == MachineState.Working)
        {
            Hint.Create("PRINTING..", 2);
        }
    }

    protected override bool IsValid(ItemType input) => input == ItemType.EmptyBook;
    public override bool PlayAnimation() => false;
    public override string GetTag() => "MachinePrinter";
}
