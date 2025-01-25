using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    [SerializeField] private List<GameObject> spawnLocations;

    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Teleport(GamePlayer gamePlayer)
    {
        Transform transformPosition = instance.spawnLocations[(int) gamePlayer.playerRole - 1].transform;
        gamePlayer.transform.position = transformPosition.position;
        gamePlayer.transform.rotation = transformPosition.rotation;
    }
}
