using Mirror;
using Steamworks;

public class GameManager : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.LobbyId, numPlayers - 1);

        var playerInfoDisplay = conn.identity.GetComponent<Player>();

        playerInfoDisplay.SetSteamId(steamId.m_SteamID);
    }

}
