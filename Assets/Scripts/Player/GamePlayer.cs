using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private RawImage profilePictureImage;
    [SerializeField] private RawImage ready, notReady;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private TMP_Text roleText;

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

        if (isLocalPlayer)
        {
            steamID = SteamUser.GetSteamID().m_SteamID;
        }

        CSteamID user = new CSteamID(steamID);
        UpdateUserInfo(user);

        if (PlayerManager.instance == null) return;
        
        PlayerManager.instance.AddGamePlayer(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }

    public void UpdateUserInfo(CSteamID user)
    {
        if (user == CSteamID.Nil) return;
        Debug.Log("SteamID" + user);
        profilePictureImage.texture = PlayerSteamUtils.GetSteamProfilePicture(user);
        displayNameText.text = PlayerSteamUtils.GetSteamUsername(user);
        roleText.text = (playerRole == PlayerRole.Factory) ? "Factory" : "Shop";
        notReady.enabled = true;
    }

    public void OnSteamIDChanged(ulong oldSteamId, ulong newSteamId)
    {
        CSteamID newCSteamID = new CSteamID(newSteamId);
        UpdateUserInfo(newCSteamID);
        /*if (LayoutManager.instance == null) return;
        LayoutManager.instance.UpdatePlayer(newCSteamID);*/
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
        roleText.text = (newRole == PlayerRole.Factory) ? "Factory" : "Shop";
    }

    public void OnChangePlayerRole(PlayerRole oldValue, PlayerRole newValue)
    {
        roleText.text = (newValue == PlayerRole.Factory) ? "Factory" : "Shop";
    }

    public void ChangeReadyStatus()
    {
        if (PlayerManager.instance.gamePlayers.Count <= 1)
        {
            LayoutManager.instance.SendColoredNotification("Second player is required!", Color.red, 3);
            return;
        }
        UpdateReadyStatus();
    }

    public void OnReadyStatusChanged(bool oldValue, bool newValue)
    {
        UpdateReadyStatus();
    }

    private void UpdateReadyStatus()
    {
        if (isReady)
        {
            notReady.enabled = false;
            ready.enabled = true;
        }
        else
        {
            ready.enabled = false;
            notReady.enabled = true;
        }

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
