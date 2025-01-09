using UnityEngine;

public class Printer : Machine
{
    public Printer() : base(MachineType.Printer) { }

    protected override void StartTimer()
    {
        actionTimer = new ActionTimer(
            () => ChangeMachineState(MachineState.Done),
            () => {
                actionTimer = null;
                ChangeMachineState(MachineState.Ready);
            }, currentRecipe.time, 1).Run();
    }

    protected override void PlaceItem(GameObject item, bool isInput) {
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

    public override bool PlayAnimation() => false;
    public override string GetTag() => "MachinePrinter";
}
