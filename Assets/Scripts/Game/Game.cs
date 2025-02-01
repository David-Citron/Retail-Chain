using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public static Game instance;

    private DateTime gameStart;

    [SerializeField] private List<GameObject> spawnLocations = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Start() {
        gameStart = DateTime.Now;
    }

    void Update() {}

    /// <summary>
    /// Teleports gameplayer to his start location
    /// </summary>
    /// <param name="gamePlayer">The gameplayer that should be teleported</param>
    public void Teleport(GamePlayer gamePlayer)
    {
        int index = (int) gamePlayer.playerRole - 1;
        Transform transformPosition = instance.spawnLocations[index].transform;
        gamePlayer.transform.position = transformPosition.position;
        gamePlayer.transform.rotation = transformPosition.rotation;
    }
}
