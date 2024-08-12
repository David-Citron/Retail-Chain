using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour 
{
    [SerializeField] public List<GamePlayer> gamePlayers = new List<GamePlayer>();
    [SerializeField] public List<TMP_Text> userNames = new List<TMP_Text>();
    [SerializeField] public List<RawImage> profilePictures = new List<RawImage>();

    [SerializeField] public Account account;

    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        gamePlayers.Clear();
        userNames.ForEach(userName => userName.text = "Player " + (userNames.IndexOf(userName) + 1));
        profilePictures.ForEach(picture => picture.texture = Texture2D.whiteTexture);
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
            GamePlayer gamePlayer = gamePlayers[i];
            if (gamePlayer.connectionId != connectionId) continue;
            if (gamePlayer.isLocalPlayer && gamePlayer.isServer) gameManager.steamLobby.LeaveLobby();

            Debug.LogWarning("Player " + i + " was removed");
            gamePlayers.Remove(gamePlayer);

            userNames[i].text = "Player " + (i + 1);
            profilePictures[i].texture = Texture2D.whiteTexture;
        }
    }
}
