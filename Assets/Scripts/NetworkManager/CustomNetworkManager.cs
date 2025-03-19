using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{

    public static CustomNetworkManager instance;
    int stopHost = -1;

    int clientsReadyCount = 0;

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
        Hint.Create("Welcome to RetailChain.", 5);

        MenuManager.instance.Open("LobbyPanel");
    }

    public override void OnClientDisconnect()
    {
        Debug.LogWarning("Client disconnected");
        base.OnClientDisconnect();

        if(SceneManager.GetActiveScene().buildIndex == 1 && Game.instance != null) Game.instance.EndTimers();
        PlayerManager.instance.Reset();

        SteamLobby.instance.LeaveLobby();
        if (LobbyHandler.instance != null) LobbyHandler.instance.HideLoadingScreen();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.LogWarning("Server callback OnServerDisconnect, connId: " + conn.connectionId);
        base.OnServerDisconnect(conn);

        if (PlayerManager.instance.gamePlayers.Count == 0) return;

        PlayerManager.instance.PlayerDisconnected(conn.connectionId); // Remove player from PlayerManager
        if(LobbyHandler.instance != null) LobbyHandler.instance.swapButton.gameObject.SetActive(false);

        clientsReadyCount = 0;

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
        clientsReadyCount = 0;
        //if (ContractManager.instance != null) ContractManager.instance.InitializeFirstContract();
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        if (SceneManager.GetActiveScene().buildIndex == 0) MenuManager.instance.Open("MainMenu"); //Automatically enable MainMenu when loading to Scene 0 (lobby)
        if (Game.instance != null) Game.instance.InitializePlayers();
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        if (SceneManager.GetActiveScene().buildIndex == 0) return;

        clientsReadyCount++;

        if (clientsReadyCount < maxConnections) return;
        PlayerManager.instance.GetLocalGamePlayer().IfPresent(player =>
        {
            if (ContractManager.instance == null) return;
            ContractManager.instance.InitializeFirstContract();
        });
    }
}