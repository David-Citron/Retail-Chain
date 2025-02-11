using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Game : NetworkBehaviour
{

    public static Game instance;

    [SerializeField] private List<GameObject> cameras;
    [SerializeField] private List<GameObject> spawnLocations; //Spawn locations for players

    [SerializeField] private List<GameObject> shopGameObjects; //All game objects that are related to Shop player -> for Factory player disable.
    [SerializeField] private List<GameObject> factoryGameObjects; //All game objects that are related to Factory player -> for Shop player disable.

    void Awake()
    {
        instance = this;
    }

    void Start() {
    }

    void Update() {}

    public void EndGame()
    {
        /*
         
        When the game ends show player his game statistics and at the bottom of it "BACK TO LOBBY"
         
         
         */
    }

    /// <summary>
    /// Teleports gameplayer to his start location
    /// </summary>
    public void InitializePlayers()
    {
        PlayerManager.instance.gamePlayers.ForEach(gamePlayer =>
        {
            if (!gamePlayer.isLocalPlayer) return;
            int index = (int)gamePlayer.playerRole - 1;

            cameras.ForEach(camera => camera.SetActive(cameras.IndexOf(camera) == index));

            Transform transformPosition = instance.spawnLocations[index].transform;
            gamePlayer.transform.position = transformPosition.position;
            gamePlayer.transform.rotation = transformPosition.rotation;

            if (gamePlayer.playerRole == PlayerRole.Shop) factoryGameObjects.ForEach(gameObject => gameObject.SetActive(false));
            else shopGameObjects.ForEach(gameObject => gameObject.SetActive(false));

            gamePlayer.StartGame();
        });
    }
}
