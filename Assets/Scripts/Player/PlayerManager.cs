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
    private List<TMP_Text> GetRolesTexts() => GetLayoutManager().roleTexts;


}
