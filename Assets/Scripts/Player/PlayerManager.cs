using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour 
{

    [SerializeField]
    public Account account;

    public List<GamePlayer> gamePlayers = new List<GamePlayer>();

    [SerializeField]
    public List<TMP_Text> userNames = new List<TMP_Text>();

    [SerializeField]
    public List<RawImage> profilePictures = new List<RawImage>();

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
        profilePictures[gamePlayers.Count].texture = gamePlayer.GetSteamProfilePicture();
        userNames[gamePlayers.Count].text = gamePlayer.GetSteamUsername();

        gamePlayers.Add(gamePlayer);
    }
}
