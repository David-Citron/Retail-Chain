using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour
{


    [SerializeField] public int connectionId = -1;

    [SyncVar(hook = nameof(OnSteamIDChanged))]
    private ulong steamID;

    [SyncVar(hook = nameof(OnChangePlayerRole))]
    public PlayerRole playerRole;

    [SyncVar(hook = nameof(OnReadyStatusChanged))]
    public bool isReady = false;

    public GameObject player;

    [SerializeField] private RawImage profilePictureImage;
    [SerializeField] private RawImage ready, notReady;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private TMP_Text roleText;
    public Material[] bodyMaterials;

    void Start()
    {
        DontDestroyOnLoad(this);

        if (isServer) connectionId = connectionToClient.connectionId;
        syncDirection = (isLocalPlayer && isServer) ? SyncDirection.ServerToClient : SyncDirection.ClientToServer;

        if (isLocalPlayer) steamID = SteamUser.GetSteamID().m_SteamID;

        CSteamID user = new CSteamID(steamID);
        UpdateUserInfo(user);

        if (PlayerManager.instance == null) return;
        
        PlayerManager.instance.AddGamePlayer(this);

        if (!isLocalPlayer) return;

        GamePlayer secondPlayer = PlayerManager.instance.GetOppositePlayer(this);
        if (secondPlayer == null) SetPlayeRole(PlayerRole.Shop);
        else SetPlayeRole(secondPlayer.playerRole == PlayerRole.Shop ? PlayerRole.Factory : PlayerRole.Shop);
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
        UpdateRoleData();
    }

    public void OnChangePlayerRole(PlayerRole oldValue, PlayerRole newValue)
    {
        UpdateRoleData();
    }

    public void OnReadyStatusChanged(bool oldValue, bool newValue)
    {
        UpdateReadyStatus();
    }

    public void ChangeReadyStatus()
    {
        isReady = !isReady;
        UpdateReadyStatus();
    }

    public void UpdateReadyStatus()
    {
        UpdateReadyIcon();
        if (!isServer) return;
        if (!isReady) return;

        var oppositePlayer = PlayerManager.instance.GetOppositePlayer(this);
        if (oppositePlayer == null) return;
        if (!oppositePlayer.isReady) return;

        NetworkManager.singleton.ServerChangeScene("Level1");

        if (!isLocalPlayer) oppositePlayer.StartGame();
        else StartGame();
    }

    private void UpdateReadyIcon()
    {
        if (isReady)
        {
            notReady.gameObject.SetActive(false);
            ready.gameObject.SetActive(true);
        }
        else
        {
            notReady.gameObject.SetActive(true);
            ready.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        Debug.Log("GAME STARTED WOHO");
    }

    private void UpdateRoleData()
    {
        Material material = bodyMaterials[playerRole == PlayerRole.Shop ? 0 : 1];

        roleText.text = (playerRole == PlayerRole.Factory) ? "Factory" : "Shop";
        roleText.color = material.color;

        Transform playerBody = player.transform.Find("Player Body");

        if (playerBody != null)
        {
            Renderer renderer = playerBody.GetComponent<Renderer>();
            if (renderer != null) renderer.material = material;
        }
    }

    public ulong GetSteamId() => steamID;
}
