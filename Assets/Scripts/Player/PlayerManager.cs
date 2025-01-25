using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour 
{
    public static PlayerManager instance;

    [SerializeField] public List<GamePlayer> gamePlayers = new List<GamePlayer>();
    [SerializeField] private List<Transform> lobbySpawnPoints = new List<Transform>();

    [SerializeField] public Account account;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (instance != null)
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
    public void Reset()
    {
        gamePlayers.Clear();
    }

    public void AddGamePlayer(GamePlayer gamePlayer)
    {
        gamePlayers.Add(gamePlayer);

        LayoutManager.Instance().IfPresent(layoutManager =>
        {
            Transform transform = lobbySpawnPoints[GetPlayerIndex(gamePlayer)];
            gamePlayer.transform.position = transform.position;
            gamePlayer.transform.rotation = transform.rotation;

            layoutManager.UpdatePlayer(new CSteamID(gamePlayer.GetSteamId()));
            GetOppositePlayer(gamePlayer).IfPresent(lobbyLeader => layoutManager.UpdateInvitePlayerButton(lobbyLeader));
        });
    }

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

        if(gamePlayers.Count != 0) LayoutManager.Instance().IfPresent(layoutManager => layoutManager.UpdateInvitePlayerButton(gamePlayers[0]));
    }

    public Optional<GamePlayer> GetOppositePlayer(GamePlayer player)
    {
        if (gamePlayers.Count < 2) return Optional<GamePlayer>.Empty();
        return Optional<GamePlayer>.Of(gamePlayers[gamePlayers.IndexOf(player) == 0 ? 1 : 0]);
    }

    public Optional<GamePlayer> GetPlayer(CSteamID id)
    {
        var index = GetPlayerIndex(id);
        return index == -1 ? Optional<GamePlayer>.Empty() : Optional<GamePlayer>.Of(gamePlayers[index]);
    }

    public Optional<GamePlayer> GetLobbyLeader() {
        var steamId = SteamMatchmaking.GetLobbyOwner(SteamLobby.LobbyId);
        foreach (var gamePlayer in gamePlayers)
        {
            if (gamePlayer.GetSteamId() != steamId.m_SteamID) continue;
            return Optional<GamePlayer>.Of(gamePlayer);
        }
        return Optional<GamePlayer>.Empty();
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

    public int GetPlayerIndex(GamePlayer gamePlayer)
    {
        return gamePlayers.IndexOf(gamePlayer);
    }

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
