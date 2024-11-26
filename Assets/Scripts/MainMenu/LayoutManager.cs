using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class LayoutManager : MonoBehaviour
{

    public static LayoutManager instance;

    [SerializeField] public List<GameObject> spawnPoints = new List<GameObject>();

    [SerializeField] public List<Button> readyButtons = new List<Button>();
    [SerializeField] public List<TMP_Text> readyTextButtons = new List<TMP_Text>();

    //Main Menu
    [SerializeField] public Button createGameButton;
    [SerializeField] public Button joinGameButton;
    [SerializeField] public RawImage mainMenuProfilePicture;
    [SerializeField] public TMP_Text mainMenuUsername;

    //Lobby Menu
    [SerializeField] public Button swapButton;
    [SerializeField] public Button leaveButton;

    [SerializeField] public TMP_Text notificationText;

    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject lobby;
    [SerializeField] public GameObject lobbiesMenu;

    [SerializeField] public GameObject steamNotInitializedNotification;


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
        var index = PlayerManager.instance.GetPlayerIndex(id);
        if (index == -1) return;
        
        var gamePlayer = PlayerManager.instance.gamePlayers[index];
        if (gamePlayer == null) return;
        
        var username = PlayerSteamUtils.GetSteamUsername(id);

        InitializeLeaveButton(gamePlayer);
        //InitializeReadyButton(gamePlayer, index);

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

    public void InitializeReadyButton(GamePlayer gamePlayer, int index)
    {
        if (!gamePlayer.isLocalPlayer) return;

        for (int i = 0; i < readyButtons.Count; i++)
        {
            var button = readyButtons[i];

            if(index != i)
            {
                button.interactable = false;
                continue;
            }

            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => gamePlayer.ChangeReadyStatus());
        }
    }

    public void InitializeRoleSwapButton(GamePlayer gamePlayer)
    {
        if (!gamePlayer.isServer)
        {
            swapButton.gameObject.SetActive(false);
            return;
        }

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
        notificationText.text = text;
        notificationText.color = color;
        yield return new WaitForSecondsRealtime(time);
        notificationText.text = "";
        notificationText.color = Color.white;
    }
}
