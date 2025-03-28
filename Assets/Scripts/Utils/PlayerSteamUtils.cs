using Steamworks;
using UnityEngine;

public class PlayerSteamUtils : MonoBehaviour {


    protected static CSteamID localPlayerSteamId;
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
    protected Callback<PersonaStateChange_t> personaStateChanged;

    private void Start()
    {
        if (!SteamManager.Initialized) return;
        localPlayerSteamId = SteamUser.GetSteamID();
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        personaStateChanged = Callback<PersonaStateChange_t>.Create(OnPersonaStateChanged);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {

        if(SteamUser.GetSteamID().m_SteamID == callback.m_steamID.m_SteamID) LobbyHandler.instance.UpdateMainMenuProfilePicture(callback.m_steamID);

        PlayerManager.instance.gamePlayers.ForEach(gamePlayer => {
            if (gamePlayer.GetSteamId() != callback.m_steamID.m_SteamID) return;
            gamePlayer.UpdateUserInfo(callback.m_steamID);
        });

        LobbiesListManager.instance.listOfLobbies.ForEach(lobbyEntry => lobbyEntry.GetComponent<LobbyDataEntry>().UpdateList());
    }

    private void OnPersonaStateChanged(PersonaStateChange_t callback)
    {
        if (MenuManager.instance != null && !MenuManager.instance.IsOpened("LobbiesList")) return;
        SteamLobby.instance.GetLobbiesList();
    }

    public static CSteamID StringToCSteamID(string steamIdString)
    {
        if (ulong.TryParse(steamIdString, out ulong steamId))
        {
            return new CSteamID(steamId);
        } else
        {
            Debug.LogError("Invalid Steam ID format.");
            return CSteamID.Nil;
        }
    }

    public static string GetSteamUsername(CSteamID steamId) => SteamFriends.GetFriendPersonaName(steamId);

    public static Texture2D GetSteamProfilePicture(CSteamID steamId)
    {
        Texture2D texture = null;
        int avatarInt = SteamFriends.GetLargeFriendAvatar(steamId);
        if (avatarInt == -1) return texture;

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
                offset -= (int)width * 4;
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

        return texture;
    }
}
