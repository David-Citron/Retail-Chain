using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby instance;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAdressKey = "HostAdressKey";

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

        if(!SteamIsInitialized()) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(onLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
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
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void onLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby was not created!");
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(LobbyId, HostAdressKey, SteamUser.GetSteamID().ToString());
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

        string hostAddress = SteamMatchmaking.GetLobbyData(LobbyId, HostAdressKey);
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
        Debug.Log("Lobby left succesfully");
    }
}
