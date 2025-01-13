public class PackagingTable : Machine
{
    public PackagingTable() : base(MachineType.PackagingTable) { }

    public override string GetTag() => "MachinePackagingTable";
    public override bool PlayAnimation() => true;
}
