using Mirror;
using Steamworks;
using UnityEngine;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSteamIDChanged))]
    private ulong steamID;

    [SyncVar(hook = nameof(OnChangePlayerRole))]
    public PlayerRole playerRole;

    [SyncVar(hook = nameof(OnReadyStatusChanged))]
    public bool isReady = false;

    private int level;
    private int experience;

    [SerializeField] public int connectionId = -1;

    void Start()
    {
        DontDestroyOnLoad(this);

        if (isServer)
        {
            connectionId = connectionToClient.connectionId;
        }

        if (isServer && isLocalPlayer || !isServer && !isLocalPlayer) playerRole = PlayerRole.Shop;
        else playerRole = PlayerRole.Factory;

        syncDirection = (isLocalPlayer && isServer) ? SyncDirection.ServerToClient : SyncDirection.ClientToServer;
        
        if (PlayerManager.instance == null)
        {
            Debug.LogWarning("PlayerManager is null");
            return;
        }

        if (isLocalPlayer)
        {
            steamID = SteamUser.GetSteamID().m_SteamID;
        }

        PlayerManager.instance.AddGamePlayer(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }

    public void OnSteamIDChanged(ulong oldSteamId, ulong newSteamId)
    {
        CSteamID newCSteamID = new CSteamID(newSteamId);
        if (LayoutManager.instance == null) return;
        LayoutManager.instance.UpdatePlayer(newCSteamID);
    }


    [ClientRpc]
    public void RpcShowUpdatedRoles()
    {
        var oppositePlayer = PlayerManager.instance.GetOppositePlayer(this);
        oppositePlayer.SetPlayeRole(playerRole);
        SetPlayeRole(playerRole == PlayerRole.Shop ? PlayerRole.Factory : PlayerRole.Shop);
    }
   

    public void SetPlayeRole(PlayerRole newRole)
    {
        playerRole = newRole;
        if (LayoutManager.instance == null) return;
        LayoutManager.instance.UpdateRoleText(new CSteamID(steamID));
    }

    public void OnChangePlayerRole(PlayerRole oldValue, PlayerRole newValue)
    {
        if (LayoutManager.instance == null) return;
        LayoutManager.instance.UpdateRoleText(new CSteamID(steamID));
    }

    public void ChangeReadyStatus()
    {
        if (PlayerManager.instance.gamePlayers.Count <= 1)
        {
            LayoutManager.instance.SendColoredNotification("Second player is required!", Color.red, 3);
            return;
        }

        isReady = !isReady;
    }

    public void OnReadyStatusChanged(bool oldValue, bool newValue)
    {
        UpdateReadyStatus();
    }

    private void UpdateReadyStatus()
    {
        if (LayoutManager.instance != null) LayoutManager.instance.UpdateReadyStatus(new CSteamID(steamID));

        if (!isServer) return;
        if (!isReady) return;

        var oppositePlayer = PlayerManager.instance.GetOppositePlayer(this);
        if (oppositePlayer == null) return;
        if (!oppositePlayer.isReady) return;

        NetworkManager.singleton.ServerChangeScene("Level1");

        if (!isLocalPlayer) oppositePlayer.StartGame();
        else StartGame();
    }

    public void StartGame()
    {
        Debug.Log("GAME STARTED WOHO");
    }

    public ulong GetSteamId() => steamID;
}
