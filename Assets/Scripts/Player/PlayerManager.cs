using Steamworks;
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
        DontDestroyOnLoad(this.gameObject);
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GamePlayer GetOppositePlayer(GamePlayer player)
    {
        if (gamePlayers.Count < 2) return null;

        return gamePlayers.IndexOf(player) == 0 ? gamePlayers[1] : gamePlayers[0];
    }

    public GamePlayer GetPlayer(CSteamID id)
    {
        var index = GetPlayerIndex(id);
        return index == -1 ? null : gamePlayers[index];
    }

    public int GetPlayerIndex(CSteamID id)
    {
        for (int i = 0; i < gamePlayers.Count; i++)
        {
            if (gamePlayers[i].GetSteamId() == id.m_SteamID) return i;
        }
        return -1;
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
        if (GetLayoutManager() == null) return;

        gamePlayers.Add(gamePlayer);
        GetLayoutManager().UpdatePlayer(new CSteamID(gamePlayer.GetSteamId()));
    }
    public void PlayerDisconnected(int connectionId)
    {
        GamePlayer gamePlayer = GetGamePlayerByConnId(connectionId);
        if(gamePlayer == null) return;

        if (gamePlayer.isLocalPlayer && gamePlayer.isServer) gameManager.steamLobby.LeaveLobby();

        Debug.Log("Player " + PlayerSteamUtils.GetSteamUsername(new CSteamID(gamePlayer.GetSteamId())) + " was removed");


        int index = gamePlayers.IndexOf(gamePlayer);

        GetUsernames()[index].text = "Player " + (index + 1);
        GetProfilePictures()[index].texture = Texture2D.whiteTexture;
        
        gamePlayers.Remove(gamePlayer);
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


}
