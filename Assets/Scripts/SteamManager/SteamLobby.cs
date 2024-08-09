using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuPanel = null;
    [SerializeField] private GameObject lobbyPanel = null;


    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyKicked_t> lobbyKicked;

    private const string HostAdressKey = "HostAdressKey";

    private NetworkManager networkManager;

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private LayoutManager layoutManager;

    public static CSteamID LobbyId {get; private set; }

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();

        if(!SteamManager.Initialized) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(onLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyKicked = Callback<LobbyKicked_t>.Create(OnLobbyExited);
    }

    public void HostLobby()
    {
        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        Debug.Log("Started hosting a lobby");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void onLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            mainMenuPanel.SetActive(true);
            lobbyPanel.SetActive(false);
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
        

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey);
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void OnLobbyExited(LobbyKicked_t callback)
    {
        Debug.LogWarning("Player kicked from the server!");
        lobbyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
