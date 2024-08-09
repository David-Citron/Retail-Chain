using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour 
{
    [SerializeField] public Account account;

    [SerializeField] public List<GamePlayer> gamePlayers = new List<GamePlayer>();
    [SerializeField] public List<TMP_Text> userNames = new List<TMP_Text>();
    [SerializeField] public List<RawImage> profilePictures = new List<RawImage>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddGamePlayer(GamePlayer gamePlayer)
    {
        gamePlayer.SetProfilePicture(profilePictures[gamePlayers.Count]);
        gamePlayer.SetUsername(userNames[gamePlayers.Count]);

        gamePlayers.Add(gamePlayer);
    }

    public void PlayerDisconnected(int connectionId)
    {
        for (int i = 0; i < gamePlayers.Count; i++)
        {
            if (gamePlayers[i].connectionToServer.connectionId == connectionId)
            {
                Debug.LogWarning("Player " + i + " was removed");
                userNames[i].text = "Player " + (i + 1);
                profilePictures[i].texture = Texture2D.whiteTexture;
            }
        }
    }
}
