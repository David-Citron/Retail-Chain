public class CutterMachine : Machine
{
    public CutterMachine() : base(MachineType.Cutter) { }

    public override bool PlayAnimation() => true;
}
