using Mirror;
using Org.BouncyCastle.Security;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSteamIDChanged))]
    [SerializeField] private ulong steamID;

    [SerializeField] private TMP_Text username = null;
    [SerializeField] private RawImage profilePicture = null;

    [SerializeField] private PlayerManager playerManager;

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
        if(avatarInt == -1) {
            Debug.LogWarning("Failed to load Steam profile picture, using default.");
            return texture;
        }

        SteamUtils.GetImageSize(avatarInt, out uint width, out uint height);
        byte[] image = new byte[width * height * 4];
        if (SteamUtils.GetImageRGBA(avatarInt, image, (int)(width * height * 4)))
        {
            texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            texture.LoadRawTextureData(image);
            texture.Apply();
        }
  
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
}
