using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IMachine
{

    GameObject GetResultPlace();
    GameObject[] GetInputPlaces();

    MachineType GetMachineType();

    List<ItemType> GetCurrentItems();
    bool IsReadyToPickUp();
}

public enum MachineType
{
    Printer,
    Workbench,
    Cutter,
    GlueDispenser
}