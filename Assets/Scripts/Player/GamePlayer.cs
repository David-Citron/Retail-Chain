using Mirror;
using Steamworks;
using System;
using UnityEngine;

public class GamePlayer : NetworkBehaviour
{

    private CSteamID steamID;

    private int level;
    private int experience;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;
        if (!isClient) return;
        CmdPlayerJoin(SteamUser.GetSteamID().ToString());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        
    }

    [Command]
    void CmdPlayerJoin(string steamId)
    {
        Debug.Log("Player with ID: " + steamId + " has joined the lobby");
    }
}
