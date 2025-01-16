using UnityEngine;

public class PackagingTable : Machine
{
    public PackagingTable() : base(MachineType.PackagingTable) { }

    protected override void ChangeMachineState(MachineState newState)
    {
        ItemType currentType = ItemType.None;
        if (newState == MachineState.Done) currentType = currentRecipe.inputs.Find(i => i != ItemType.Package);

        base.ChangeMachineState(newState);

        if (resultItem == null) return;
        Debug.LogError(resultItem.name);
        ItemManager.UpdateItem(resultItem, currentType);
    }

    public override string GetTag() => "MachinePackagingTable";
    public override bool PlayAnimation() => true;
}