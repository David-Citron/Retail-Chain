using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby instance;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    protected Callback<LobbyMatchList_t> lobbyList;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;

    public List<CSteamID> lobbyIds = new List<CSteamID>();


    public const string HostCSteamIDKey = "HostCSteamID";

    private NetworkManager networkManager;

    [SerializeField] private PlayerManager playerManager;

    public static CSteamID LobbyId {get; private set; }

    private bool steamIsInitialized;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        networkManager = GetComponent<NetworkManager>();

        if (!SteamIsInitialized()) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    private bool SteamIsInitialized()
    {
        steamIsInitialized = SteamManager.Initialized;
        if (!steamIsInitialized) LayoutManager.instance.ShowSteamNotInitializedNotification();
        return steamIsInitialized;
    }

    public void HostLobby()
    {
        Debug.Log("Started hosting a lobby");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
    }

    public void JoinLobby(CSteamID steamID)
    {
        SteamMatchmaking.JoinLobby(steamID);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby was not created!");
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(LobbyId, HostCSteamIDKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(LobbyId, "host-name", PlayerSteamUtils.GetSteamUsername(SteamUser.GetSteamID()));
        SteamMatchmaking.SetLobbyData(LobbyId, "game", "retailchain");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;

        CSteamID lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        LobbyId = lobbyId;

        string hostAddress = SteamMatchmaking.GetLobbyData(LobbyId, HostCSteamIDKey);
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }

    public void LeaveLobby()
    {
        Debug.Log("Leaving lobby " + LobbyId);
        SteamMatchmaking.LeaveLobby(LobbyId);

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);
        }

        if (LayoutManager.instance == null) return;
        LayoutManager.instance.ShowMainMenu();
    }

    public void GetLobbiesList()
    {
        if(lobbyIds.Count > 0) lobbyIds.Clear();
        SteamMatchmaking.AddRequestLobbyListStringFilter("game", "retailchain", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbiesListManager.instance.listOfLobbies.Count > 0) LobbiesListManager.instance.DestroyLobbies();
        else LobbiesListManager.instance.StopDisplayingLobbies();

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIds.Add(lobbyId);
            SteamMatchmaking.RequestLobbyData(lobbyId);
            Debug.Log(i);
        }
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.instance.DisplayLobbies(lobbyIds, result);
    }
}
