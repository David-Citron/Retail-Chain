using Mirror;
using Steamworks;
using System;
using UnityEngine;

public class GamePlayer : NetworkBehaviour
{

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
        if (isServer && isLocalPlayer) RpcSendHello();
    }

    private void FixedUpdate()
    {
        
    }

    [Command]
    void CmdPlayerJoin(string steamId)
    {
        Debug.Log("Player with ID: " + steamId + " has joined the lobby");
    }

    [ClientRpc]
    void RpcSendHello()
    {
        Debug.Log("Hello everybody!!!");
    }
}
