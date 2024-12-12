using System.Collections.Generic;
using UnityEngine;

public interface IMachine
{

    List<GameObject> GetCurrentGameObjects();

    GameObject GetResultPlace();
    GameObject[] GetInputPlaces();

    MachineType GetMachineType();
    MachineState GetMachineState();
}

public enum MachineType
{
    Printer,
    Workbench,
    Cutter,
    GlueDispenser
}