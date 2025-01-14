using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour
{

    [SerializeField] public int connectionId = -1;

    [SyncVar(hook = nameof(OnSteamIDChanged))]
    private ulong steamID;

    [SyncVar(hook = nameof(OnChangePlayerRole))]
    public PlayerRole playerRole = PlayerRole.Unassigned;

    [SyncVar(hook = nameof(OnReadyStatusChanged))]
    public bool isReady = false;

    public PlayerBank bankAccount;

    public GameObject player; //GamePlayer's prefab.

    [SerializeField] private RawImage profilePictureImage;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private GameObject ready;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private Material[] bodyMaterials;
    [SerializeField] private RawImage lobbyLeaderCrown;
    [SerializeField] private Button kickButton;

    private Scene scene;

    private void Awake()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        scene = SceneManager.GetActiveScene();
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        if (isLocalPlayer) steamID = SteamUser.GetSteamID().m_SteamID;
        if (isServer) connectionId = connectionToClient.connectionId;
        if (isServer && isLocalPlayer || !isServer && !isLocalPlayer) syncDirection = SyncDirection.ServerToClient;
        else syncDirection = SyncDirection.ClientToServer;

        if (isLocalPlayer) steamID = SteamUser.GetSteamID().m_SteamID;

        UpdateUserInfo(new CSteamID(steamID)); //Updates username, profile picture.

        if (PlayerManager.instance != null) PlayerManager.instance.AddGamePlayer(this);

        InicializeButtons();

        LayoutManager.Instance().IfPresent(layoutManager =>
        {
            layoutManager.SendNotification("Player " + PlayerSteamUtils.GetSteamUsername(new CSteamID(steamID)) + " has joined your Lobby.", 5);
            if (isLocalPlayer) layoutManager.HideLoadingScreen();
        });

        if (!isLocalPlayer) return;

        //If there is no second player, the PlayerRole is set to Shop, otherwise it depends on the role of the opposite player.
        PlayerManager.instance.GetOppositePlayer(this).IfPresentOrElse(secondPlayer =>
        SetPlayeRole(secondPlayer.playerRole == PlayerRole.Shop ? PlayerRole.Factory : PlayerRole.Shop), () => SetPlayeRole(PlayerRole.Shop));
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
        profilePictureImage.texture = PlayerSteamUtils.GetSteamProfilePicture(user);
        displayNameText.text = PlayerSteamUtils.GetSteamUsername(user);
        ready.SetActive(false);
    }

    public void OnSteamIDChanged(ulong oldSteamId, ulong newSteamId)
    {
        CSteamID newCSteamID = new CSteamID(newSteamId);
        UpdateUserInfo(newCSteamID);
    }

    [ClientRpc]
    public void RpcShowUpdatedRoles()
    {
        PlayerManager.instance.GetOppositePlayer(this).IfPresent(oppositePlayer =>
        {   
            oppositePlayer.SetPlayeRole(playerRole);
            SetPlayeRole(playerRole == PlayerRole.Shop ? PlayerRole.Factory : PlayerRole.Shop);
        });
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
        if (isServer && isLocalPlayer) return;
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

        var oppositePlayer = PlayerManager.instance.GetOppositePlayer(this).GetValueOrDefault();
        if (oppositePlayer == null || !oppositePlayer.isReady) return;

        NetworkManager.singleton.ServerChangeScene("Level1");
    }

    private void UpdateReadyIcon()
    {
        ready.SetActive(isReady);
        if(isReady) GetComponent<LobbyAnimator>().playReadyAnimation();

        if (!isLocalPlayer) return;
        LayoutManager.Instance().IfPresent(layoutManager =>
        {
            layoutManager.readyButton.gameObject.SetActive(!isReady);
            layoutManager.readyCancelButton.gameObject.SetActive(isReady);
        });
    }

    public void StartGame()
    {
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);

        if(isLocalPlayer)
        {
            gameObject.GetComponent<PlayerBank>().enabled = true;

            player.AddComponent<PlayerInputManager>().enabled = true;
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<PlayerPickUp>().enabled = true;

            player.transform.eulerAngles = new Vector3(0, 0, 0);
            player.transform.position = Vector3.zero;
        } else
        {
            player.gameObject.SetActive(false);
        }

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

    /// <summary>
    /// Inicialize buttons in lobby (kick & swap roles)
    /// </summary>
    private void InicializeButtons()
    {
        lobbyLeaderCrown.gameObject.SetActive(isServer && isLocalPlayer || !isServer && !isLocalPlayer);

        kickButton.gameObject.SetActive(isServer && !isLocalPlayer);
        if (isServer && !isLocalPlayer)
        {
            kickButton.interactable = true;
            kickButton.onClick.RemoveAllListeners();
            kickButton.onClick.AddListener(() =>
            {
                CustomNetworkManager.instance.KickPlayer(connectionId);
            });
        }

        LayoutManager.Instance().IfPresent(layoutManager =>
        {
            var swapButton = layoutManager.swapButton;
            swapButton.gameObject.SetActive(isServer && !isLocalPlayer);
            if (isServer && !isLocalPlayer)
            {
                swapButton.interactable = true;
                swapButton.onClick.RemoveAllListeners();
                swapButton.onClick.AddListener(() =>
                {
                    if (isReady || PlayerManager.instance.GetOppositePlayer(this).GetValueOrDefault().isReady)
                    {
                        LayoutManager.Instance().IfPresent(layoutManager => layoutManager.SendColoredNotification("One of the player is already ready!", Color.red, 3));
                        return;
                    }

                    RpcShowUpdatedRoles();
                });
            }
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var oppositePlayer = PlayerManager.instance.GetOppositePlayer(this).GetValueOrDefault();
        if (oppositePlayer == null)
        {
            //KICK TO LOBBY
            return;
        }

        StartGame();
    }
    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public ulong GetSteamId() => steamID;
}
