using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDataEntry : MonoBehaviour
{

    public CSteamID lobbyId;
    public CSteamID hostId;

    public RawImage profilePicture;
    public TMP_Text username;
    public Button joinButton;
    void Start() {}
    void Update() {}

    public void UpdateList()
    {
        if (hostId == null) return;
        SteamFriends.RequestUserInformation(hostId, false);

        profilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(hostId);
        username.text = PlayerSteamUtils.GetSteamUsername(hostId);


        joinButton.enabled = true;
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            LobbyHandler.instance.ShowLoadingScreen();
            SteamLobby.instance.JoinLobby(lobbyId);
        });
    }
}
