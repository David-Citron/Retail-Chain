using Mirror;
using Steamworks;
using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GamePlayer : NetworkBehaviour
{

    private CSteamID steamID;

    private PlayerManager playerManager;

    private int level;
    private int experience;


    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;
        if (!isClient) return;
        steamID = SteamUser.GetSteamID();
        CmdPlayerJoin(SteamUser.GetSteamID().ToString());

        playerManager = (PlayerManager) FindAnyObjectByType(typeof(PlayerManager));
        if(playerManager == null) return;
        playerManager.AddGamePlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        
    }

    [Command]
    void CmdPlayerJoin(string steamId)
    {
        Debug.Log("Player with ID: " + steamId + " has joined the lobby");
    }


    public string GetSteamUsername()
    {
        return isLocalPlayer ? SteamFriends.GetPersonaName() : SteamFriends.GetFriendPersonaName(steamID);
    }

    public Texture2D GetSteamProfilePicture()
    {


        Debug.Log("Loading img texture for: " + steamID);

        Texture2D texture = null;
        int avatarInt = SteamFriends.GetLargeFriendAvatar(steamID);
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
}
