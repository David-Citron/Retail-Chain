using System.Collections.Generic;
using UnityEngine;

public interface IMachine
{

    List<GameObject> GetCurrentGameObjects();
    List<GameObject> GetInputPlaces();

    GameObject GetResultPlace();

    MachineType GetMachineType();
    MachineState GetMachineState();

    bool PlayAnimation();
}

public enum MachineType
{
    Printer,
    Workbench,
    Cutter,
    GlueDispenser,
    PackagingTable
}