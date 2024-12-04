using Steamworks;
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
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    /*private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        throw new NotImplementedException();
    }*/

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

    public int GetPlayerIndex(GamePlayer gamePlayer)
    {
        return gamePlayers.IndexOf(gamePlayer);
    }

    public void Reset()
    {
        gamePlayers.Clear();
    }

    public void AddGamePlayer(GamePlayer gamePlayer)
    {
        gamePlayers.Add(gamePlayer);

        if (LayoutManager.instance == null) return;

        gamePlayer.transform.position = lobbySpawnPoints[GetPlayerIndex(gamePlayer)].position;
        gamePlayer.transform.rotation = lobbySpawnPoints[GetPlayerIndex(gamePlayer)].rotation;

        LayoutManager.instance.UpdatePlayer(new CSteamID(gamePlayer.GetSteamId()));
    }

    public void PlayerDisconnected(int connectionId)
    {
        GamePlayer gamePlayer = GetGamePlayerByConnId(connectionId);
        if(gamePlayer == null) return;

        Debug.Log("Player " + PlayerSteamUtils.GetSteamUsername(new CSteamID(gamePlayer.GetSteamId())) + " has disconnected.");
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

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0) return;
        GameObject spawnPoint1 = GameObject.Find("PlayerSpawnPoint1");
        GameObject spawnPoint2 = GameObject.Find("PlayerSpawnPoint1");
        lobbySpawnPoints.Add(spawnPoint1.transform);
        lobbySpawnPoints.Add(spawnPoint2.transform);
    }
}
