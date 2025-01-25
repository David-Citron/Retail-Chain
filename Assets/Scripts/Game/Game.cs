using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public static Game instance;

    [SerializeField] private List<GameObject> spawnLocations = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Start() {}

    void Update() {}

    public void Teleport(GamePlayer gamePlayer)
    {
        int index = (int) gamePlayer.playerRole - 1;
        Transform transformPosition = instance.spawnLocations[index].transform;
        gamePlayer.transform.position = transformPosition.position;
        gamePlayer.transform.rotation = transformPosition.rotation;
    }
}
