using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineManager : MonoBehaviour
{

    private static MachineManager instance;

    public List<Machine> machines = new List<Machine>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
    }

    // Update is called once per frame
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
