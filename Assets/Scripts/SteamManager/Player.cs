using Mirror;
using Steamworks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [SyncVar(hook = nameof(HandleSteamUpdated))]
    private ulong steamId;

    [SerializeField] private TMP_Text displayName = null;

    private int level;
    private double experience;

    #region Server

    public void SetSteamId(ulong steamId)
    {
        this.steamId = steamId;
    }


    #endregion



    #region Client
    private void HandleSteamUpdated(ulong oldSteamId, ulong newSteamId)
    {
        var cSteamId = new CSteamID(newSteamId);
        displayName.text = SteamFriends.GetFriendPersonaName(cSteamId);

    }

    #endregion
}