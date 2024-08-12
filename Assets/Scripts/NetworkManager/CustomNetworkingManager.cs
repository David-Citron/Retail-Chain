using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkingManager : NetworkManager
{
    private GameManager gameManager;
    private LayoutManager layoutManager;
    private PlayerManager playerManager;

    public override void Start()
    {
        base.Start();
        gameManager = GameManager.Instance;
        layoutManager = gameManager.layoutManager;
        playerManager = gameManager.playerManager;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        layoutManager.ShowLobby();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.LogWarning("Client disconnected");
        layoutManager.ShowMainMenu();
        gameManager.steamLobby.LeaveLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.LogWarning("Server disconnected");
        if (!isNetworkActive) layoutManager.ShowMainMenu();
        playerManager.PlayerDisconnected(conn.connectionId);
    }
}
