using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Scene scene;

    public CustomNetworkManager networkManager;
    public LayoutManager layoutManager;
    public SteamLobby steamLobby;
    public Account account;
    public PlayerManager playerManager;

    private void Start()
    {
        Instance = this;

        if (SceneManager.GetActiveScene().buildIndex != 0) return;
        layoutManager.InicializeHostButton();
    }

    public void SetScene(Scene newScene)
    {
        this.scene = newScene;
    }
}
