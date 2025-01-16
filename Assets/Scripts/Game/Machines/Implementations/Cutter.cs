using UnityEngine;

public class Cutter : Machine
{

    [SerializeField] private GameObject blade;
    [SerializeField] private int rotationSpeed = 25;

    public Cutter() : base(MachineType.Cutter) {}

    protected override void Update()
    {
        base.Update();
        if (machineState != MachineState.Working) return;
        blade.transform.rotation *= Quaternion.Euler(rotationSpeed * Time.deltaTime, 0, 0);
    }

    protected override void ChangeMachineState(MachineState newState)
    {
        base.ChangeMachineState(newState);
    }

    public override string GetTag() => "MachineCutter";
    public override bool PlayAnimation() => true;
}
