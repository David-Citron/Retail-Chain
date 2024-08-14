using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour 
{
    [SerializeField] public List<GamePlayer> gamePlayers = new List<GamePlayer>();

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

    public void ChangeReadyStatus(GamePlayer gamePlayer)
    {
        gamePlayer.ChangeReadyStatus();
    }

    public void Reset()
    {
        gamePlayers.Clear();
        GetUsernames().ForEach(userName => userName.text = "Player " + (GetUsernames().IndexOf(userName) + 1));
        GetProfilePictures().ForEach(picture => picture.texture = Texture2D.whiteTexture);
    }

    public void AddGamePlayer(GamePlayer gamePlayer)
    {
        var index = gamePlayers.Count;

        gamePlayer.SetProfilePicture(GetProfilePictures()[index]);
        gamePlayer.SetUsername(GetUsernames()[index]);
        gamePlayer.SetReadyStatus(GetReadyButtons()[index], GetReadyTextButtons()[index], false);

        for (int i = 0; i < GetReadyButtons().Count; i++)
        {
            var currentButton = GetReadyButtons()[i];

            gamePlayer.InitializeReadyButton(currentButton, currentButton == GetReadyButtons()[index]);
        }

        gamePlayer.InitializeLeaveButton(GetLayoutManager().leaveButton);

        gamePlayers.Add(gamePlayer);
    }

    /**
     * 
     * returns gameplayer object that left.
     *
     *
     */
    public GamePlayer PlayerDisconnected(int connectionId)
    {
        for (int i = 0; i < gamePlayers.Count; i++)
        {
            GamePlayer gamePlayer = gamePlayers[i];
            if (gamePlayer.connectionId != connectionId) continue;
            if (gamePlayer.isLocalPlayer && gamePlayer.isServer) gameManager.steamLobby.LeaveLobby();

            Debug.LogWarning("Player " + i + " was removed");
            gamePlayers.Remove(gamePlayer);

            GetUsernames()[i].text = "Player " + (i + 1);
            GetProfilePictures()[i].texture = Texture2D.whiteTexture;
            return gamePlayer;
        }
        return null;
    }

    public GamePlayer GetGamePlayerByConnId(int connId)
    {

        for (int i = 0; i < gamePlayers.Count; i++)
        {
            GamePlayer gamePlayer = gamePlayers[i];
            if (gamePlayer.connectionId != connId) continue;
            return gamePlayer;
        }
        return null;
    }
    public LayoutManager GetLayoutManager() => gameManager.layoutManager;
    private List<TMP_Text> GetUsernames() => GetLayoutManager().userNames;
    private List<RawImage> GetProfilePictures() => GetLayoutManager().profilePictures;
    private List<TMP_Text> GetReadyTextButtons() => GetLayoutManager().readyTextButtons;
    private List<Button> GetReadyButtons() => GetLayoutManager().readyButtons;
    
}
