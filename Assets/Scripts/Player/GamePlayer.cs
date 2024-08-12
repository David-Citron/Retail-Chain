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


    private Button readyButton = null;
    private TMP_Text ready = null;

    private TMP_Text username = null;
    private RawImage profilePicture = null;

    private PlayerManager playerManager;

    private bool isReady = false;

    private int level;
    private int experience;

    [SerializeField] public int connectionId = -1;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    void Start()
    {
        if (isServer) connectionId = connectionToClient.connectionId;

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

   public void OnSteamIDChanged(ulong oldSteamId, ulong newSteamId)
    {
        if(username == null || profilePicture == null) { return; }

        CSteamID newCSteamID = new CSteamID(newSteamId);
        username.text = GetSteamUsername(newCSteamID);
        profilePicture.texture = GetSteamProfilePicture(newCSteamID);
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
            Debug.Log("imageReceived = " + BitConverter.ToString(imageReceived));
            byte[] image = new byte[width * height * 4];
            int nextByte = 0;
            for (int i = 0; i < (int)height; i++)
            {
                for (int j = 0; j < (int)width; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        image[nextByte] = imageReceived[nextByte];
                        nextByte++;
                    }
                }
            }
            Debug.Log("image = " + BitConverter.ToString(image));
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

    public void SetUsername(TMP_Text text)
    {
        Debug.Log("Username object set for " + steamID);
        username = text;
        text.text = GetSteamUsername(new CSteamID(steamID));
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
        if (isLocalPlayer && authority)
        {
            button.interactable = true;
            button.onClick.AddListener(ChangeReadyStatus);
            return;
        }

        button.interactable = false;
    }

    public void SetReadyStatus(Button button, TMP_Text text)
    {
        ready = text;
        readyButton = button;

        text.text = isReady ? "READY" : "NOT READY";
        text.color = isReady ? Color.green : Color.red;
    }

    public void ChangeReadyStatus()
    {
        isReady = !isReady;
        SetReadyStatus(readyButton, ready);
    }
}
