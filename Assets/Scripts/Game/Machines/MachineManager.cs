using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineManager : MonoBehaviour
{

    private static MachineManager instance;

    public List<Machine> machines = new List<Machine>();

    void Start()
    {
        instance = this;
        
    }

    void Update()
    {
        
    }

    public static bool IsAnyWorking()
    {
        bool result  = false;
        Instance().IfPresent(machineManager =>
        {
            result = machineManager.machines.Any(machine => machine.GetMachineState() == MachineState.Working);
        });
        return result;
    }

    public static bool IsGameObjectInMachine(GameObject gameObject)
    {
        bool result = false;
        Instance().IfPresent(machineManager =>
        {
            result = machineManager.machines.Find(machine => machine.GetMachineState() == MachineState.Working && machine.GetCurrentGameObjects().Contains(gameObject)) != null;
        });
        return result;
    }

    public static Optional<MachineManager> Instance() => Optional<MachineManager>.Of(instance);
}
