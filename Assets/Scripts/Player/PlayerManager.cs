using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour 
{
    public static PlayerManager instance;

    public List<GamePlayer> gamePlayers = new List<GamePlayer>();
    [SerializeField] private List<Transform> lobbySpawnPoints = new List<Transform>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Update() {}

    /// <summary>
    /// Clears gameplayers.
    /// </summary>
    public void Reset() => gamePlayers.Clear();
    
    /// <summary>
    /// Adds gameplayer to list, while it also create a player object on lobby and set ups buttons.
    /// </summary>
    /// <param name="gamePlayer">The gameplayer</param>
    public void AddGamePlayer(GamePlayer gamePlayer)
    {
        gamePlayers.Add(gamePlayer);
        Transform transform = lobbySpawnPoints[GetPlayerIndex(gamePlayer)];
        gamePlayer.transform.position = transform.position;
        gamePlayer.transform.rotation = transform.rotation;

        LobbyHandler.instance.UpdatePlayer(gamePlayer);
    }

    /// <summary>
    /// Disconnects player and will assure that game wont start.
    /// </summary>
    /// <param name="connectionId">The connection id of disconnected player</param>
    public void PlayerDisconnected(int connectionId)
    {
        GetGamePlayerByConnId(connectionId).IfPresent(gamePlayer =>
        {
            GetOppositePlayer(gamePlayer).IfPresent(oppositePlayer => {
                    if(oppositePlayer.isReady) oppositePlayer.ChangeReadyStatus();
                });

            Debug.Log("Player " + PlayerSteamUtils.GetSteamUsername(new CSteamID(gamePlayer.GetSteamId())) + " has disconnected.");
            gamePlayers.Remove(gamePlayer);
        });

        if (gamePlayers.Count != 0 && LobbyHandler.instance != null) LobbyHandler.instance.UpdateInvitePlayerButton(gamePlayers[0]);
    }

    /// <summary>
    /// Gets the opposite player of given player
    /// </summary>
    /// <param name="player">The gameplayer</param>
    /// <returns>The Optional of opposite gameplayer object</returns>
    public Optional<GamePlayer> GetOppositePlayer(GamePlayer player)
    {
        if (gamePlayers.Count < 2) return Optional<GamePlayer>.Empty();
        return Optional<GamePlayer>.Of(gamePlayers[gamePlayers.IndexOf(player) == 0 ? 1 : 0]);
    }

    /// <summary>
    /// Gets GamePlayer object based on given CSteamID
    /// </summary>
    /// <param name="id">CSteamID of player</param>
    /// <returns>The option of GamePlayer</returns>
    public Optional<GamePlayer> GetPlayer(CSteamID id)
    {
        for (int i = 0; i < gamePlayers.Count; i++)
        {
            if (gamePlayers[i].GetSteamId() != id.m_SteamID) continue;
            return Optional<GamePlayer>.Of(gamePlayers[i]);
        }
        return Optional<GamePlayer>.Empty();
    }

    /// <summary>
    /// Gets lobby leader.
    /// </summary>
    /// <returns>The optional of lobby leader gameplayer object</returns>
    public Optional<GamePlayer> GetLobbyLeader() {
        var steamId = SteamMatchmaking.GetLobbyOwner(SteamLobby.LobbyId);
        foreach (var gamePlayer in gamePlayers)
        {
            if (gamePlayer.GetSteamId() != steamId.m_SteamID) continue;
            return Optional<GamePlayer>.Of(gamePlayer);
        }
        return Optional<GamePlayer>.Empty();
    }


    /// <summary>
    /// Gets index of given gameplayer
    /// </summary>
    /// <param name="gamePlayer">The gameplayer</param>
    /// <returns>The index of gameplayer in list.</returns>
    public int GetPlayerIndex(GamePlayer gamePlayer) => gamePlayers.IndexOf(gamePlayer);
    

    /// <summary>
    /// Gets gameplayer by connection id.
    /// </summary>
    /// <param name="connId">The connection id of gameplayer</param>
    /// <returns>The optional of gameplayer object.</returns>
    public Optional<GamePlayer> GetGamePlayerByConnId(int connId)
    {

        for (int i = 0; i < gamePlayers.Count; i++)
        {
            GamePlayer gamePlayer = gamePlayers[i];
            if (gamePlayer.connectionId != connId) continue;
            return Optional<GamePlayer>.Of(gamePlayer);
        }
        return Optional<GamePlayer>.Empty();
    }

    /// <summary>
    /// Gets local gameplayer object.
    /// </summary>
    /// <returns>The gameobject of local player.</returns>
    public Optional<GamePlayer> GetLocalGamePlayer()
    {
        for (int i = 0; i < gamePlayers.Count; i++)
        {
            GamePlayer gamePlayer = gamePlayers[i];
            if (!gamePlayer.isLocalPlayer) continue;
            return Optional<GamePlayer>.Of(gamePlayer);
        }
        return Optional<GamePlayer>.Empty();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0) return;
        lobbySpawnPoints.Clear();

        GameObject spawnPoint1 = GameObject.Find("PlayerSpawnPoint1");
        GameObject spawnPoint2 = GameObject.Find("PlayerSpawnPoint2");
        lobbySpawnPoints.Add(spawnPoint1.transform);
        lobbySpawnPoints.Add(spawnPoint2.transform);
    }
}
