using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class LayoutManager : MonoBehaviour
{

    public static LayoutManager instance;

    [SerializeField] public List<TMP_Text> userNames = new List<TMP_Text>();
    [SerializeField] public List<RawImage> profilePictures = new List<RawImage>();

    [SerializeField] public List<Button> readyButtons = new List<Button>();
    [SerializeField] public List<TMP_Text> readyTextButtons = new List<TMP_Text>();

    [SerializeField] public List<TMP_Text> roleTexts = new List<TMP_Text>();

    [SerializeField] public Button leaveButton;
    [SerializeField] public Button swapButton;
    [SerializeField] public Button hostButton;

    [SerializeField] private GameObject defaultButtonsGroup;
    [SerializeField] private GameObject playButtonsGroup;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject steamNotInitializedNotification;

    [SerializeField] private TMP_Text notificationText;

    void Start()
    {
        instance = this;
        defaultButtonsGroup.SetActive(true);
        playButtonsGroup.SetActive(false);
        lobby.SetActive(false);
    }

    public void UpdatePlayer(CSteamID id)
    {
        var index = GameManager.Instance.playerManager.GetPlayerIndex(id);
        if (index == -1) return;

        var gamePlayer = GameManager.Instance.playerManager.gamePlayers[index];
        if (gamePlayer == null) return;

        var username = PlayerSteamUtils.GetSteamUsername(id);

        profilePictures[index].texture = PlayerSteamUtils.GetSteamProfilePicture(id);
        userNames[index].text = username;

        UpdateRoleText(gamePlayer, index);

        InitializeLeaveButton(gamePlayer);
        InitializeReadyButton(gamePlayer, index);
        InitializeRoleSwapButton(gamePlayer);

        SendNotification("Player " + username + " has joined your Lobby.", 5);
    }

    public void InicializeHostButton()
    {
        hostButton.onClick.AddListener(() =>
        {
            SteamLobby.instance.HostLobby();
        });
    }

    public void UpdateRoleText(CSteamID id)
    {
        var index = GameManager.Instance.playerManager.GetPlayerIndex(id);
        if (index == -1) return;
        var gamePlayer = GameManager.Instance.playerManager.gamePlayers[index];
        if (gamePlayer == null) return;

        UpdateRoleText(gamePlayer, index);
    }

    public void UpdateRoleText(GamePlayer gamePlayer, int index)
    {
        var status = gamePlayer.isReady;
        roleTexts[index].text = gamePlayer.playerRole.ToString();
    }

    public void UpdateReadyStatus(CSteamID id)
    {
        var index = GameManager.Instance.playerManager.GetPlayerIndex(id);
        if (index == -1) return;

        var gamePlayer = GameManager.Instance.playerManager.gamePlayers[index];
        if (gamePlayer == null) return;

        var text = readyTextButtons[index];
        text.text = gamePlayer.isReady ? "READY" : "NOT READY";
        text.color = gamePlayer.isReady ? Color.green : Color.red;
    }

    public void InitializeLeaveButton(GamePlayer gamePlayer)
    {
        leaveButton.interactable = true;
        leaveButton.onClick.AddListener(() =>
        {
            if (gamePlayer.isServer && gamePlayer.isClient) GameManager.Instance.networkManager.StopHost();
            else GameManager.Instance.networkManager.StopClient();

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

        var playerManager = GameManager.Instance.playerManager;
        if (playerManager == null) return;

        swapButton.onClick.AddListener(() =>
        {
            if (playerManager.gamePlayers.Count == 1)
            {
                SendColoredNotification("Second player is required!", Color.red, 3);
                return;
            }

            gamePlayer.RpcShowUpdatedRoles();
        });
    }

    public void ChangeActive(GameObject button)
    {
        button.SetActive(!button.activeInHierarchy);
    }

    public void BackToMainMenu(GameObject current)
    {
        defaultButtonsGroup.SetActive(true);
        current.SetActive(false);
    }

    public void ShowLobby()
    {
        lobby.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        lobby.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Quit game");
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
        Debug.Log("New notification: " + text + " for " + time);
        notificationText.text = text;
        notificationText.color = color;
        yield return new WaitForSecondsRealtime(time);
        notificationText.text = "";
        notificationText.color = Color.white;
    }
}
