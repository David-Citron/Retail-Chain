using Mirror;
using Org.BouncyCastle.Security;
using Steamworks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSteamIDChanged))]
    private ulong steamID;

    [SyncVar(hook = nameof(OnChangePlayerRole))]
    private PlayerRole playerRole;

    private TMP_Text playerRoleText = null;

    private TMP_Text usernameText = null;
    private RawImage profilePicture = null;

    private PlayerManager playerManager;

    [SyncVar(hook = nameof(OnReadyStatusChanged))]
    private bool isReady = false;

    private Button readyButton = null;
    private TMP_Text readyText = null;

    private int level;
    private int experience;

    [SerializeField] public int connectionId = -1;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

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
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }

    public string GetSteamUsername(CSteamID newSteamId)
    {
        return isLocalPlayer ? SteamFriends.GetPersonaName() : SteamFriends.GetFriendPersonaName(newSteamId);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID != steamID) return;
        profilePicture.texture = GetSteamProfilePicture(callback.m_steamID);
    }

    public Texture2D GetSteamProfilePicture(CSteamID newSteamId)
    {
        Debug.Log("Loading img texture for: " + newSteamId);

        Texture2D texture = null;
        int avatarInt = SteamFriends.GetLargeFriendAvatar(newSteamId);
        if (avatarInt == -1)
        {
            Debug.LogWarning("Failed to load Steam profile picture, using default.");
            return texture;
        }

        SteamUtils.GetImageSize(avatarInt, out uint width, out uint height);
        byte[] imageReceived = new byte[width * height * 4];
        if (SteamUtils.GetImageRGBA(avatarInt, imageReceived, (int)(width * height * 4)))
        {
            byte[] image = new byte[width * height * 4];
            int saveTo;
            int loadFrom = 0;
            int offset = imageReceived.Length;
            for (int i = 0; i < (int)height; i++)
            {
                offset-=(int)width * 4;
                saveTo = offset; 
                for (int j = 0; j < (int)width; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        image[saveTo] = imageReceived[loadFrom];
                        saveTo++;
                        loadFrom++;
                    }
                }
            }

            texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            texture.LoadRawTextureData(image);
            texture.Apply();
        }
        else Debug.LogError("SteamUtils didn't give back ImageRGBA");

        return texture;
    }

    public void SetProfilePicture(RawImage image)
    {
        Debug.Log("Profile picture object set for " + steamID);
        profilePicture = image;
        image.texture = GetSteamProfilePicture(new CSteamID(steamID));
    }

    public void SetPlayerRoleText(TMP_Text text)
    {
        playerRoleText = text;
        text.text = playerRole.ToString();
    }

    public void SetUsername(TMP_Text text)
    {
        Debug.Log("Username object set for " + steamID);
        usernameText = text;
        text.text = GetSteamUsername(new CSteamID(steamID));
    }

    public void OnSteamIDChanged(ulong oldSteamId, ulong newSteamId)
    {
        if (usernameText == null || profilePicture == null) return;

        CSteamID newCSteamID = new CSteamID(newSteamId);
        usernameText.text = GetSteamUsername(newCSteamID);
        profilePicture.texture = GetSteamProfilePicture(newCSteamID);
        GameManager.Instance.layoutManager.SendNotification("New bitch there " + usernameText.text + ".", 5);
    }

    public void InitializeLeaveButton(Button button)
    {
        button.interactable = true;
        button.onClick.AddListener(() =>
        {
            Debug.Log(isServer + " " + isClient);
            if (isServer && isClient) GameManager.Instance.networkManager.StopHost();
            else GameManager.Instance.networkManager.StopClient();
            

            playerManager.GetLayoutManager().ShowMainMenu();
        });
    }

    public void InitializeReadyButton(Button button, bool authority)
    {
        if (!isLocalPlayer && !authority) return;

        if (isLocalPlayer && authority)
        {
            button.interactable = true;
            button.onClick.AddListener(ChangeReadyStatus);
            return;
        }

        button.interactable = false;
    }

    public void InitializeRoleSwapButton(Button button)
    {
        if (!isServer) {
            button.gameObject.SetActive(false);
            return;
        }

        if (!isLocalPlayer) return;

        button.onClick.AddListener(() =>
        {
            if (playerManager.gamePlayers.Count == 1)
            {
                playerManager.GetLayoutManager().SendColoredNotification("Second player is required!", Color.red, 3);
                return;
            }

            var oppositePlayer = playerManager.GetOppositePlayer(this);
            oppositePlayer.SetPlayeRole(playerRole);
            SetPlayeRole(playerRole == PlayerRole.Shop ? PlayerRole.Factory : PlayerRole.Shop);

            Debug.Log(oppositePlayer.GetSteamUsername(new CSteamID(oppositePlayer.GetSteamId())) + " (opposite player) was set to " + oppositePlayer.playerRole.ToString());
            Debug.Log(GetSteamUsername(new CSteamID(GetSteamId())) + " was set to " + playerRole.ToString());
        });
    }


    public void SetPlayeRole(PlayerRole newRole)
    {
        playerRole = newRole;
        playerRoleText.text = newRole.ToString();
    }

    public void OnChangePlayerRole(PlayerRole oldValue, PlayerRole newValue)
    {
        if (playerRoleText == null) return;
        playerRoleText.text = newValue.ToString();
    }

    public void ChangeReadyStatus()
    {
        isReady = !isReady;
        SetReadyStatus(readyButton, readyText, isReady);
    }

    public void OnReadyStatusChanged(bool oldValue, bool newValue)
    {
        SetReadyStatus(readyButton, readyText, newValue);
    }

    public void SetReadyStatus(Button button, TMP_Text text, bool status)
    {
        if(readyText != text) readyText = text;
        if(readyButton != button) readyButton = button;

        text.text = status ? "READY" : "NOT READY";
        text.color = status ? Color.green : Color.red;
    }

    public ulong GetSteamId() => steamID;
}
