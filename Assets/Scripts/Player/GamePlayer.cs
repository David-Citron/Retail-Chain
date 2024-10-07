using Mirror;
using Org.BouncyCastle.Security;
using Steamworks;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSteamIDChanged))]
    private ulong steamID;

    [SyncVar(hook = nameof(OnChangePlayerRole))]
    public PlayerRole playerRole;

    private PlayerManager playerManager;

    [SyncVar(hook = nameof(OnReadyStatusChanged))]
    public bool isReady = false;

    private int level;
    private int experience;

    [SerializeField] public int connectionId = -1;

    void Start()
    {
        if (isServer)
        {
            connectionId = connectionToClient.connectionId;
        }

        if (isServer && isLocalPlayer || !isServer && !isLocalPlayer) playerRole = PlayerRole.Shop;
        else playerRole = PlayerRole.Factory;

        syncDirection = (isLocalPlayer && isServer) ? SyncDirection.ServerToClient : SyncDirection.ClientToServer;
        
        playerManager = (PlayerManager) FindAnyObjectByType(typeof(PlayerManager));
        if (playerManager == null)
        {
            Debug.LogWarning("PlayerManager is null");
            return;
        }

        if (isLocalPlayer)
        {
            steamID = SteamUser.GetSteamID().m_SteamID;
        }

        playerManager.AddGamePlayer(this);
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
        if (GameManager.Instance.layoutManager == null) return;
        GameManager.Instance.layoutManager.UpdatePlayer(newCSteamID);
    }


    [ClientRpc]
    public void RpcShowUpdatedRoles()
    {
        var oppositePlayer = playerManager.GetOppositePlayer(this);
        oppositePlayer.SetPlayeRole(playerRole);
        SetPlayeRole(playerRole == PlayerRole.Shop ? PlayerRole.Factory : PlayerRole.Shop);
    }
   

    public void SetPlayeRole(PlayerRole newRole)
    {
        playerRole = newRole;
        if (GameManager.Instance.layoutManager == null) return;
        GameManager.Instance.layoutManager.UpdateRoleText(new CSteamID(steamID));
    }

    public void OnChangePlayerRole(PlayerRole oldValue, PlayerRole newValue)
    {
        if (GameManager.Instance.layoutManager == null) return;
        GameManager.Instance.layoutManager.UpdateRoleText(new CSteamID(steamID));
    }

    public void ChangeReadyStatus()
    {
        if (playerManager.gamePlayers.Count <= 1)
        {
            playerManager.GetLayoutManager().SendColoredNotification("Second player is required!", Color.red, 3);
            return;
        }

        isReady = !isReady;
        UpdateReadyStatus();
    }

    public void OnReadyStatusChanged(bool oldValue, bool newValue)
    {
        UpdateReadyStatus();
    }

    private void UpdateReadyStatus()
    {
        CheckReadyStatus();
        if (GameManager.Instance.layoutManager == null) return;
        GameManager.Instance.layoutManager.UpdateReadyStatus(new CSteamID(steamID));
    }

    public void CheckReadyStatus()
    {
       
        if (!isServer) return;

        if (!isReady) return;

        var oppositePlayer = playerManager.GetOppositePlayer(this);
        if(oppositePlayer == null) return;
        if (!oppositePlayer.isReady) return;

        if (!isLocalPlayer) oppositePlayer.StartGame();
        else StartGame();
    }

    public void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("Level1");

        Debug.Log("GAME STARTED WOHO");
    }

    public ulong GetSteamId() => steamID;
}
