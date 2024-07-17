using Mirror;
using System;
using UnityEngine;

public class GamePlayer : NetworkBehaviour
{

    private int level;
    private int experience;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player " + NetworkClient.connection.connectionId + " has joined on Lobby " + SteamLobby.LobbyId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
