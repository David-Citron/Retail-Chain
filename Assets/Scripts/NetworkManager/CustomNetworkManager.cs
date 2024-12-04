using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public class CustomNetworkManager : NetworkManager
{

    public static CustomNetworkManager instance;
    int stopHost = -1;

    public override void Start()
    {
        instance = this;
        base.Start();
    }

    public override void OnClientConnect()
    {
        Debug.LogWarning("Client connect.");
        base.OnClientConnect();

        PlayerManager.instance.Reset();

        LayoutManager.Instance().IfPresent(layoutManager =>
        {
            layoutManager.ShowLobby();
            layoutManager.SendColoredNotification("Welcome to RetailChain.", Color.green, 5);
        });
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
        LayoutManager.Instance().IfPresent(layoutManager => layoutManager.kickButton.gameObject.SetActive(false));

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

    public void KickPlayer(int connectionId)
    {
        if(NetworkServer.connections.TryGetValue(connectionId, out NetworkConnectionToClient connection))
        {
            connection.Disconnect();
        }
        Debug.Log("kicked " + connection);
    }

    public override void OnStartHost()
    {
        stopHost = -1;
        base.OnStartHost();
    }
}