public class Cutter : Machine
{
    public Cutter() : base(MachineType.Cutter) { }

    public override string GetTag() => "MachineCutter";
    public override bool PlayAnimation() => true;
}
