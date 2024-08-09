using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetSteamID))]
    private ulong steamID;

    [SerializeField] private TMP_Text username = null;
    [SerializeField] private RawImage profilePicture = null;

    private PlayerManager playerManager;

    private int level;
    private int experience;


    // Start is called before the first frame update
    void Start()
    {
        playerManager = (PlayerManager) FindAnyObjectByType(typeof(PlayerManager));
        if (playerManager == null)
        {
            Debug.LogWarning("PlayerManager is null");
            return;
        }

        playerManager.AddGamePlayer(this);
        if (isLocalPlayer) steamID = (ulong) SteamUser.GetSteamID();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        
    }

    public void SetSteamID(ulong oldSteamId, ulong newSteamId)
    {
        CSteamID newCSteamID = new CSteamID(newSteamId);
        Debug.Log("SteamID change: From " + oldSteamId + " to " + newSteamId);
        username.text = GetSteamUsername(newCSteamID);
        profilePicture.texture = GetSteamProfilePicture(newCSteamID);
    }

    [Command]
    void CmdPlayerJoin(string steamId)
    {
        Debug.Log("Player with ID: " + steamId + " has joined the lobby");
    }


    public string GetSteamUsername(CSteamID newSteamId)
    {
        return isLocalPlayer ? SteamFriends.GetPersonaName() : SteamFriends.GetFriendPersonaName(newSteamId);
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
    }

    public void SetUsername(TMP_Text text)
    {
        Debug.Log("Username object set for " + steamID);
        username = text;
    }
}
