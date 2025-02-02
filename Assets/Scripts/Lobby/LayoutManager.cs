using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class LayoutManager : MonoBehaviour
{

    private static LayoutManager instance;

    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();

    //Menus
    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject lobby;
    [SerializeField] public GameObject lobbiesMenu;
    [SerializeField] private GameObject settingsMenu;


    //Main Menu
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private RawImage mainMenuProfilePicture;
    [SerializeField] private TMP_Text mainMenuUsername;

    //Lobby Menu
    [SerializeField] public Button swapButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] public Button readyButton;
    [SerializeField] public Button readyCancelButton;

    [SerializeField] private Button lobbyType;
    [SerializeField] private Button inviteFriend;


    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private GameObject steamNotInitializedNotification;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject loadingScreenPrefab;
    private GameObject activeLoadingScreen;

    void Start()
    {
        instance = this;

        ShowMainMenu();
        InicializeMainMenuButtons();

        if (!SteamManager.Initialized) return;
        UpdateMainMenuProfilePicture(SteamUser.GetSteamID());
    }

    /// <summary>
    /// Updates player's data (profile image, username) in MainMenu scene. Initialize buttons.
    /// </summary>
    /// <param name="id">CSteamID of player</param>
    public void UpdatePlayer(CSteamID id)
    {
        if (id == CSteamID.Nil) return;
        PlayerManager.instance.GetPlayer(id).IfPresent(gamePlayer =>
        {
            InitializeLeaveButton(gamePlayer);
            InitializeReadyButtons(gamePlayer);
            InitializeLobbyTypeButtons(gamePlayer);
            UpdateInvitePlayerButton(gamePlayer);
        });
    }

    /// <summary>
    /// Adds listener to the host button to properly host lobby.
    /// </summary>
    private void InicializeMainMenuButtons()
    {
        if (createGameButton != null) {
            createGameButton.interactable = true;
            createGameButton.onClick.RemoveAllListeners();
            createGameButton.onClick.AddListener(() =>
            {
                SteamLobby.instance.HostLobby();
                ShowLoadingScreen();
            });
        }

        if(joinGameButton != null) {
            joinGameButton.interactable = true;
            joinGameButton.onClick.RemoveAllListeners();
            joinGameButton.onClick.AddListener(() =>
            {
                mainMenu.SetActive(false);
                lobby.SetActive(false);
                lobbiesMenu.SetActive(true);
                LobbiesListManager.instance.DestroyLobbies();
                StartCoroutine(LobbiesListManager.instance.UpdateLobbyList());
            });
        }
    }

    public void UpdateMainMenuProfilePicture(CSteamID id)
    {
        if(id == null)
        {
            Debug.LogWarning("id cannot be null");
            return;
        }

        mainMenuProfilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(id);
        mainMenuUsername.text = PlayerSteamUtils.GetSteamUsername(id);
    }

    private void InitializeLeaveButton(GamePlayer gamePlayer)
    {
        leaveButton.onClick.RemoveAllListeners();
        leaveButton.interactable = true;
        leaveButton.onClick.AddListener(() =>
        {
            if (gamePlayer.isServer && gamePlayer.isClient) CustomNetworkManager.singleton.StopHost();
            else CustomNetworkManager.singleton.StopClient();

            ShowMainMenu();
        });
    }

    private void InitializeReadyButtons(GamePlayer gamePlayer)
    {
        if (!gamePlayer.isLocalPlayer) return;

        readyButton.interactable = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(() => {
            if(PlayerManager.instance.gamePlayers.Count <= 1)
            {
                SendColoredNotification("Both players are required to start the game!", Color.red, 3);
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
        if(!gamePlayer.isServer && !gamePlayer.isLocalPlayer) return;

        inviteFriend.gameObject.SetActive(PlayerManager.instance.gamePlayers.Count == 1);
        inviteFriend.interactable = true;
        inviteFriend.onClick.RemoveAllListeners();
        inviteFriend.onClick.AddListener(() =>
        {
            SteamFriends.ActivateGameOverlay("friends");
            SteamFriends.ActivateGameOverlayRemotePlayTogetherInviteDialog(SteamLobby.LobbyId);
        });
    }

    public void ShowLobby()
    {
        lobby.SetActive(true);
        mainMenu.SetActive(false);
        lobbiesMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        lobby.SetActive(false);
        lobbiesMenu.SetActive(false);
        if (settingsMenu.activeInHierarchy) settingsMenu.GetComponent<Settings>().Close();
        settingsMenu.SetActive(false);
    }

    public void ShowSettings()
    {
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<Settings>().Open();
        mainMenu.SetActive(false);
        lobby.SetActive(false);
        lobbiesMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowSteamNotInitializedNotification()
    {
        steamNotInitializedNotification.SetActive(true);
    }

    public void SendNotification(string text, int time)
    {
        SendColoredNotification(text, Color.white, time);
    }

    public void SendColoredNotification(string text, Color color, int time)
    {
        StartCoroutine(SendEnumaratorNotification(text, color, time));
    }

    public IEnumerator SendEnumaratorNotification(string text, Color color, int time)
    {
        notificationText.enabled = true;
        notificationText.text = text;
        notificationText.color = color;
        yield return new WaitForSecondsRealtime(time);
        notificationText.text = "";
        notificationText.color = Color.white;
        notificationText.enabled = false;
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

    public static Optional<LayoutManager> Instance() => instance == null ? Optional<LayoutManager>.Empty() : Optional<LayoutManager>.Of(instance);
}
