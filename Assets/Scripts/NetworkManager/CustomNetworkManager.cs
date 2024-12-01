using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public class CustomNetworkManager : NetworkManager
{
    int stopHost = -1;

    public override void Start()
    {
        base.Start();
    }

    public override void OnClientConnect()
    {
        Debug.LogWarning("Client connect.");
        base.OnClientConnect();

        PlayerManager.instance.Reset();

        if(LayoutManager.instance != null)
        {
            LayoutManager.instance.ShowLobby();
            LayoutManager.instance.SendColoredNotification("Welcome to RetailChain.", Color.green, 5);
        }
    }

    public override void OnClientDisconnect()
    {
        Debug.LogWarning("Client disconnected");
        base.OnClientDisconnect();

        SteamLobby.instance.LeaveLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.LogWarning("Server callback OnServerDisconnect, connId: " + conn.connectionId);
        base.OnServerDisconnect(conn);

        if (PlayerManager.instance.gamePlayers.Count == 0) return;

        PlayerManager.instance.PlayerDisconnected(conn.connectionId); // Remove player from PlayerManager

        // Handle Lobby disconnect - Stop hosting once the host leaves
        if (PlayerManager.instance.gamePlayers.Count == 0 && SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (conn.connectionId == stopHost) return;
            Debug.LogWarning("All players have left the lobby - Stopping Host");
            stopHost = conn.connectionId;
            StopHost();
            return;
        }
        // Handle In-game disconnect - Stop host once any player leaves
        if (PlayerManager.instance.gamePlayers.Count == 1 && SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (conn.connectionId == stopHost) return;
            Debug.LogWarning("One player left the lobby - Stopping Host");
            stopHost = conn.connectionId;
            StopHost();
            return;
        }
    }

    public override void OnStartHost()
    {
        stopHost = -1;
        base.OnStartHost();
    }
}