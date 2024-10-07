using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
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

        playerManager.Reset();
        layoutManager.ShowLobby();

        gameManager.layoutManager.SendColoredNotification("Welcome to RetailChain.", Color.green, 5);
        Debug.LogWarning("Client connect.");
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SceneManager.LoadScene(0);
        Debug.LogWarning("Client disconnected");
        gameManager.steamLobby.LeaveLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.LogWarning("Server disconnected");

        playerManager.PlayerDisconnected(conn.connectionId);

        if (!isNetworkActive) layoutManager.ShowMainMenu();
    }
}
