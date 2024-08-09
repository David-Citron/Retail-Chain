using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LayoutManager layoutManager;
    public SteamLobby steamLobby;
    public Account account;
    public PlayerManager playerManager;

    private void Start()
    {
        Instance = this;
    }
}
