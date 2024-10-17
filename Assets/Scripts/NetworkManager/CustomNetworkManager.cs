using UnityEngine;
using Mirror;

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
        base.OnClientConnect();

        PlayerManager.instance.Reset();
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

        PlayerManager.instance.PlayerDisconnected(conn.connectionId);
    }
}
