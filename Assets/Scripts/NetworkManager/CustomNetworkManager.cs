using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public class CustomNetworkManager : NetworkManager
{
    private GameManager gameManager;

    public override void Start()
    {
        base.Start();
        gameManager = GameManager.Instance;
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
        Debug.LogWarning("Server disconnected");
        base.OnServerDisconnect(conn);

        PlayerManager.instance.PlayerDisconnected(conn.connectionId);

        //This is being called only on Server side, so when the 2nd player leaves, we need to stop the server.
        if (PlayerManager.instance.gamePlayers.Count == 0) return; //If the list is empty we do not want to StopHost - player is already stopped
        StopHost();
    }
}