using Mirror;
using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour 
{
    public static PlayerManager instance;

    [SerializeField] public List<GamePlayer> gamePlayers = new List<GamePlayer>();

    [SerializeField] public Account account;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
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
            if (gamePlayers[i].GetSteamId() != id.m_SteamID) continue;
            return i;
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

        if (LayoutManager.instance == null) return;
        GetUsernames().ForEach(userName => userName.text = "Player " + (GetUsernames().IndexOf(userName) + 1));
        GetProfilePictures().ForEach(picture => picture.texture = Texture2D.whiteTexture);
    }

    public void AddGamePlayer(GamePlayer gamePlayer)
    {
        gamePlayers.Add(gamePlayer);

        if (LayoutManager.instance == null) return;
        

        LayoutManager.instance.UpdatePlayer(new CSteamID(gamePlayer.GetSteamId()));
    }

    public void PlayerDisconnected(int connectionId)
    {
        GamePlayer gamePlayer = GetGamePlayerByConnId(connectionId);
        if(gamePlayer == null) return;


        Debug.Log("Player " + PlayerSteamUtils.GetSteamUsername(new CSteamID(gamePlayer.GetSteamId())) + " has disconnected.");

        int index = gamePlayers.IndexOf(gamePlayer);
        gamePlayers.Remove(gamePlayer);

        if (LayoutManager.instance == null) return;

        GetUsernames()[index].text = "Player " + (index + 1);
        GetProfilePictures()[index].texture = Texture2D.whiteTexture;
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

    private List<TMP_Text> GetUsernames() => LayoutManager.instance.userNames;
    private List<RawImage> GetProfilePictures() => LayoutManager.instance.profilePictures;


}
