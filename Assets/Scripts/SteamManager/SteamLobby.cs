using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby instance;
    public static CSteamID LobbyId { get; private set; }
    public static ELobbyType lobbyType { get; private set; }

    public const ELobbyType DEFAULT_LOBBY_TYPE = ELobbyType.k_ELobbyTypePublic;
    public const string HostCSteamIDKey = "HostCSteamID";

    private NetworkManager networkManager;

    private List<CSteamID> lobbyIds = new List<CSteamID>();

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    protected Callback<LobbyMatchList_t> lobbyList;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;

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
        SteamAPI.Init();
        steamIsInitialized = SteamManager.Initialized;
        if (!steamIsInitialized) MenuManager.instance.ToggleUI("SteamNotInitializedNotification");
        return steamIsInitialized;
    }

    public void HostLobby()
    {
        Debug.Log("Started hosting a lobby");
        lobbyType = DEFAULT_LOBBY_TYPE;
        SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
        LobbyHandler.instance.ShowLoadingScreen();
    }

    public void JoinLobby(CSteamID steamID) => SteamMatchmaking.JoinLobby(steamID);
    

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
        SteamMatchmaking.SetLobbyData(LobbyId, "game", "retailchain");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) => SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

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

        if(MenuManager.instance != null) MenuManager.instance.Open("MainMenu");

        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        SceneManager.LoadScene(0);
    }

    public void GetLobbiesList()
    {
        if (lobbyIds.Count > 0) lobbyIds.Clear();
        SteamMatchmaking.AddRequestLobbyListStringFilter("game", "retailchain", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        LobbiesListManager.instance.DestroyLobbies();

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIds.Add(lobbyId);
            SteamMatchmaking.RequestLobbyData(lobbyId);
        }
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.instance.DisplayLobbies(lobbyIds, result);
    }

    public void ChangeLobbyType(ELobbyType eLobbyType)
    {
        if (lobbyType == eLobbyType) return;
        bool status = SteamMatchmaking.SetLobbyType(LobbyId, eLobbyType);
        if (status)
        {
            lobbyType = eLobbyType;
            Debug.LogWarning("Lobby type changed! New lobby type is: " + lobbyType);
        }else
        {
            Debug.LogError("Lobby type was NOT changed! Lobby still remains with type: " + lobbyType);
        }
    }
}
