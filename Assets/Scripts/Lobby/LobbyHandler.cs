using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{

    public static LobbyHandler instance;

    //Main Menu
    [SerializeField] private RawImage mainMenuProfilePicture;
    [SerializeField] private TMP_Text mainMenuUsername;

    //Lobby Menu
    [SerializeField] public Button swapButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] public Button readyButton;
    [SerializeField] public Button readyCancelButton;
    [SerializeField] private Button lobbyType;
    [SerializeField] private Button inviteFriend;


    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject loadingScreenPrefab;
    private GameObject activeLoadingScreen;

    void Start()
    {
        instance = this;

        if (!SteamManager.Initialized) return;
        UpdateMainMenuProfilePicture(SteamUser.GetSteamID());
    }

    void Update() {}

    public void UpdateMainMenuProfilePicture(CSteamID id)
    {
        if (id == null)
        {
            Debug.LogWarning("id cannot be null");
            return;
        }

        mainMenuProfilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(id);
        mainMenuUsername.text = PlayerSteamUtils.GetSteamUsername(id);
    }

    /// <summary>
    /// Updates player's data (profile image, username) in MainMenu scene. Initialize buttons.
    /// </summary>
    /// <param name="id">CSteamID of player</param>
    public void UpdatePlayer(GamePlayer gamePlayer)
    {
        InitializeLeaveButton(gamePlayer);
        InitializeReadyButtons(gamePlayer);
        InitializeLobbyTypeButtons(gamePlayer);
        UpdateInvitePlayerButton(gamePlayer);
    }

    private void InitializeLeaveButton(GamePlayer gamePlayer)
    {
        leaveButton.onClick.RemoveAllListeners();
        leaveButton.interactable = true;
        leaveButton.onClick.AddListener(() =>
        {
            if (gamePlayer.isServer && gamePlayer.isClient) CustomNetworkManager.singleton.StopHost();
            else CustomNetworkManager.singleton.StopClient();
            MenuManager.instance.Open("MainMenu");
        });
    }

    private void InitializeReadyButtons(GamePlayer gamePlayer)
    {
        if (!gamePlayer.isLocalPlayer) return;

        readyButton.interactable = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(() => {
            if (PlayerManager.instance.gamePlayers.Count <= 1)
            {
                Hint.Create("Both players are required to start the game!", Color.red, 3);
                return;
            }
            gamePlayer.ChangeReadyStatus();
        });

        readyCancelButton.interactable = true;
        readyCancelButton.onClick.RemoveAllListeners();
        readyCancelButton.onClick.AddListener(() => {
            gamePlayer.ChangeReadyStatus();
        });
    }

    private void InitializeLobbyTypeButtons(GamePlayer gamePlayer)
    {
        lobbyType.gameObject.SetActive(gamePlayer.isServer);

        if (!gamePlayer.isServer && !gamePlayer.isLocalPlayer) return;

        lobbyType.GetComponentInChildren<TMP_Text>().text = "Lobby type: " + (SteamLobby.lobbyType == ELobbyType.k_ELobbyTypePublic ? "PUBLIC" : "PRIVATE");

        lobbyType.interactable = true;
        lobbyType.onClick.RemoveAllListeners();
        lobbyType.onClick.AddListener(() =>
        {
            ELobbyType current = SteamLobby.lobbyType;
            SteamLobby.instance.ChangeLobbyType(current == ELobbyType.k_ELobbyTypePublic ? ELobbyType.k_ELobbyTypePrivate : ELobbyType.k_ELobbyTypePublic);
            lobbyType.GetComponentInChildren<TMP_Text>().text = "Lobby type: " + (current == ELobbyType.k_ELobbyTypePublic ? "PRIVATE" : "PUBLIC");
        });
    }

    public void UpdateInvitePlayerButton(GamePlayer gamePlayer)
    {
        if (!gamePlayer.isServer && !gamePlayer.isLocalPlayer) return;

        inviteFriend.gameObject.SetActive(PlayerManager.instance.gamePlayers.Count == 1);
        inviteFriend.interactable = true;
        inviteFriend.onClick.RemoveAllListeners();
        inviteFriend.onClick.AddListener(() =>
        {
            SteamFriends.ActivateGameOverlay("friends");
            SteamFriends.ActivateGameOverlayRemotePlayTogetherInviteDialog(SteamLobby.LobbyId);
        });
    }

    /// <summary>
    /// Spawns the loading screen.
    /// </summary>
    public void ShowLoadingScreen()
    {
        if (activeLoadingScreen != null || loadingScreenPrefab == null) return;
        activeLoadingScreen = Instantiate(loadingScreenPrefab);

        activeLoadingScreen.transform.SetParent(canvas.transform, false);

    }

    /// <summary>
    /// Destroys the loading screen.
    /// </summary>
    public void HideLoadingScreen()
    {
        if (activeLoadingScreen != null)
        {
            Destroy(activeLoadingScreen);
            activeLoadingScreen = null;
        }
    }
}
