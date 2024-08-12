using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

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
        playerManager.Reset();

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
    }

    public void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(LobbyId);
    }
}
