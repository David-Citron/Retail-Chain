using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{

    public static CustomNetworkManager instance;
    int stopHost = -1;

    public override void Start()
    {
        if (instance != null) return;
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

        PlayerManager.instance.Reset();

        SteamLobby.instance.LeaveLobby();
        LayoutManager.Instance().IfPresent(layoutManager => layoutManager.HideLoadingScreen());
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.LogWarning("Server callback OnServerDisconnect, connId: " + conn.connectionId);
        base.OnServerDisconnect(conn);

        if (PlayerManager.instance.gamePlayers.Count == 0) return;

        PlayerManager.instance.PlayerDisconnected(conn.connectionId); // Remove player from PlayerManager
        LayoutManager.Instance().IfPresent(layoutManager => {
            layoutManager.swapButton.gameObject.SetActive(false);
        });

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
    }

    public override void OnStartHost()
    {
        stopHost = -1;
        base.OnStartHost();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (ContractManager.instance != null) ContractManager.instance.InitializeFirstContract();
        if (Game.instance != null) Game.instance.InitializePlayers();
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        PlayerManager.instance.GetLocalGamePlayer().IfPresent(player =>
        {
            if (conn != player.connectionToClient)
            {
                if (ContractManager.instance == null) return;
                ContractManager.instance.InitializeFirstContract();
            }
        });
    }
}