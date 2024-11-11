using Steamworks;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateList()
    {
        if (hostId == null) return;
        profilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(hostId);
        username.text = PlayerSteamUtils.GetSteamUsername(hostId);

        joinButton.interactable = true;
        joinButton.enabled = true;
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            JoinLobby();
        });
    }

    public void JoinLobby()
    {
        SteamLobby.instance.JoinLobby(lobbyId);
    }
}
