using System.Collections.Generic;
using UnityEngine;

public class CutterMachine : Machine
{
    private List<GameObject> currentItems = new List<GameObject>();

    [SerializeField] private GameObject resultPlace;

    private bool isReadyToPickUp;

    public CutterMachine() : base(MachineType.Cutter) { }

    void Start()
    {

    }

    void Update()
    {

    }
}
