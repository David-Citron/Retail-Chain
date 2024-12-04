using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class LayoutManager : MonoBehaviour
{

    public static LayoutManager instance;

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
    [SerializeField] private Button swapButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button readyCancelButton;
    [SerializeField] public Button kickButton;

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
        GamePlayer gamePlayer = PlayerManager.instance.GetPlayer(id);
        if (gamePlayer == null) return;
        
        var username = PlayerSteamUtils.GetSteamUsername(id);

        InitializeLeaveButton(gamePlayer);
        InitializeReadyButtons(gamePlayer);
        InitializeRoleSwapButton(gamePlayer);

        SendNotification("Player " + username + " has joined your Lobby.", 5);
    }

    /// <summary>
    /// Adds listener to the host button to properly host lobby.
    /// </summary>
    public void InicializeMainMenuButtons()
    {
        if (createGameButton != null) {
            createGameButton.interactable = true;
            createGameButton.onClick.RemoveAllListeners();
            createGameButton.onClick.AddListener(() =>
            {
                SteamLobby.instance.HostLobby();
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

    public void InitializeLeaveButton(GamePlayer gamePlayer)
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

    public void InitializeReadyButtons(GamePlayer gamePlayer)
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
            readyButton.gameObject.SetActive(false);
            readyCancelButton.gameObject.SetActive(true);
            gamePlayer.ChangeReadyStatus();
        });

        readyCancelButton.interactable = true;
        readyCancelButton.onClick.RemoveAllListeners();
        readyCancelButton.onClick.AddListener(() => {
            readyButton.gameObject.SetActive(true);
            readyCancelButton.gameObject.SetActive(false);
            gamePlayer.ChangeReadyStatus();
        });
    }

    public void InitializeRoleSwapButton(GamePlayer gamePlayer)
    {
        swapButton.gameObject.SetActive(gamePlayer.isServer);

        if (!gamePlayer.isServer) return;
        if (!gamePlayer.isLocalPlayer) return;

        var playerManager = PlayerManager.instance;
        if (playerManager == null) return;

        swapButton.onClick.RemoveAllListeners();
        swapButton.onClick.AddListener(() =>
        {
            if (playerManager.gamePlayers.Count == 1)
            {
                SendColoredNotification("Second player is required!", Color.red, 3);
                return;
            }

            if(gamePlayer.isReady || playerManager.GetOppositePlayer(gamePlayer).isReady)
            {
                SendColoredNotification("One of the player is already ready!", Color.red, 3);
                return;
            }

            gamePlayer.RpcShowUpdatedRoles();
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
        settingsMenu.SetActive(false);
    }

    public void ShowSettings()
    {
        settingsMenu.SetActive(true);
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
        Debug.Log("showing");
        if (activeLoadingScreen != null || loadingScreenPrefab == null) return;

            Debug.Log("SHOWED");
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
