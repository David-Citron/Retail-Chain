public class Workbench : Machine
{
    public Workbench() : base(MachineType.Workbench) {}

    public override string GetTag() => "MachineWorkbench";
    public override bool PlayAnimation() => true;
}
