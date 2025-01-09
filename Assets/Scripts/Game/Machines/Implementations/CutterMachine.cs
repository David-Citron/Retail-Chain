public class CutterMachine : Machine
{
    public CutterMachine() : base(MachineType.Cutter) { }

    public override string GetTag() => "MachineCutter";
    public override bool PlayAnimation() => true;
}
