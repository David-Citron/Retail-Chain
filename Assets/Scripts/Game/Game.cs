using Steamworks;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    public static Game instance;

    public static List<ActionTimer> timers;

    [SerializeField] private TMP_Text username;
    [SerializeField] private RawImage profilePicture;
    [SerializeField] private TMP_Text balance;

    [SerializeField] private List<GameObject> cameras;
    [SerializeField] private List<GameObject> spawnLocations; //Spawn locations for players

    [SerializeField] private List<GameObject> shopGameObjects; //All game objects that are related to Shop player -> for Factory player disable.
    [SerializeField] private List<GameObject> factoryGameObjects; //All game objects that are related to Factory player -> for Shop player disable.

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("You can't create another instance!");
            return;
        }
        instance = this;
        timers = new List<ActionTimer>();

        if (SteamAPI.Init())
        {
            CSteamID id = SteamUser.GetSteamID();
            if (id != CSteamID.Nil)
            {
                username.text = PlayerSteamUtils.GetSteamUsername(id);
                profilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(id);
            }
        }
        Interactable.interactions = new List<Interaction>();
        shopGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(false));
        factoryGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(false));
    }

    void Start() {
        if (PlayerManager.instance == null)
        {
            int numberOfPlayersFound = FindObjectsOfType<PlayerMovement>().Length;
            if (numberOfPlayersFound == 1)
            {
                GameObject testPlayer = FindFirstObjectByType<PlayerMovement>().gameObject;
                if (testPlayer != null) testPlayer.SetActive(true);
            }

            shopGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(true));
            factoryGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(true));
            return;
        }
        PlayerMovement[] scripts = FindObjectsOfType<PlayerMovement>();
        for (int i = 0; i < FindObjectsOfType<PlayerMovement>().Length; i++)
        {
            if (scripts[i].transform.GetComponentInParent<GamePlayer>() == null)
            {
                Debug.LogWarning("Test player deleted");
                Destroy(scripts[i].gameObject);
            }
        }
        GamePlayer localPlayer = PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault();
        if (localPlayer == null) return;
        if(localPlayer.playerRole == PlayerRole.Shop) shopGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(true));
        else factoryGameObjects.ForEach (currentGameObject => currentGameObject.SetActive(true));
        
    }

    void Update() {}

    public void EndGame()
    {
        EndTimers();
        MenuManager.instance.Open("GameOver");

        /*
         
        When the game ends show player his game statistics and at the bottom of it "BACK TO LOBBY"
         
         
         */
    }

    public void EndTimers() => timers.ForEach(timer => timer.Stop());
    

    /// <summary>
    /// Teleports gameplayer to his start location
    /// </summary>
    public void InitializePlayers()
    {
        PlayerManager.instance.gamePlayers.ForEach(gamePlayer =>
        {
            gamePlayer.StartGame();

            int index = (int) gamePlayer.playerRole - 1;

            Transform transformPosition = spawnLocations[index].transform;
            gamePlayer.transform.SetPositionAndRotation(transformPosition.position, transformPosition.rotation);

            if (!gamePlayer.isLocalPlayer) return;

            cameras.ForEach(camera => camera.SetActive(cameras.IndexOf(camera) == index));

            if (gamePlayer.playerRole == PlayerRole.Shop) factoryGameObjects.ForEach(currentGameObject => Destroy(currentGameObject));
            else shopGameObjects.ForEach(currentGameObject => Destroy(currentGameObject));
        });
    }

    public void UpdateBalance(int amount) => balance.text = "$" + amount.ToString("N0", CultureInfo.InvariantCulture);
    public void UpdateBalance() => UpdateBalance(PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault().bankAccount.GetBalance());
}
