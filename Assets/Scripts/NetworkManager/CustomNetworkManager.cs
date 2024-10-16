using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    private GameManager gameManager;
    private PlayerManager playerManager;

    public override void Start()
    {
        base.Start();
        gameManager = GameManager.Instance;
        playerManager = gameManager.playerManager;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        playerManager.Reset();
        if(LayoutManager.instance != null)
        {
            LayoutManager.instance.ShowLobby();
            LayoutManager.instance.SendColoredNotification("Welcome to RetailChain.", Color.green, 5);
        }
        Debug.LogWarning("Client connect.");
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        Debug.LogWarning("Client disconnected");
        SteamLobby.instance.LeaveLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.LogWarning("Server disconnected");

        playerManager.PlayerDisconnected(conn.connectionId);

        if (!isNetworkActive) LayoutManager.instance.ShowMainMenu();
    }
}
