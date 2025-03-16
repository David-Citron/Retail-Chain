using Steamworks;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using FontStyles = TMPro.FontStyles;

public class Game : MonoBehaviour
{

    public static Game instance;

    public static List<ActionTimer> timers;

    [SerializeField] private TMP_Text username;
    [SerializeField] private RawImage profilePicture;
    [SerializeField] private TMP_Text balance;
    [SerializeField] private GameObject balanceInfo;
    [SerializeField] private TMP_FontAsset fontAsset;

    [SerializeField] private List<GameObject> cameras;
    [SerializeField] private List<GameObject> spawnLocations; //Spawn locations for players

    [SerializeField] private List<GameObject> shopGameObjects; //All game objects that are related to Shop player -> for Factory player disable.
    [SerializeField] private List<GameObject> factoryGameObjects; //All game objects that are related to Factory player -> for Shop player disable.

    [SerializeField] private List<GameObject> forceOnForTestPlayer;
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
    }

    void Start() {
        if (PlayerManager.instance == null)
        {
            int numberOfPlayersFound = FindObjectsOfType<PlayerMovement>().Length;
            if (numberOfPlayersFound == 1)
            {
                GameObject testPlayer = FindFirstObjectByType<PlayerMovement>().gameObject;
                if (testPlayer != null) testPlayer.SetActive(true);
                float distanceToFactory = Vector3.Distance(testPlayer.transform.position, new Vector3(-10, 0, 0));
                float distanceToShop = Vector3.Distance(testPlayer.transform.position, new Vector3(10, 0, 0));
                if (distanceToFactory < distanceToShop)
                {
                    shopGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(false));
                    factoryGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(true));
                    cameras[0].SetActive(false);
                    cameras[1].SetActive(true); 
                }
                else
                {
                    shopGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(true));
                    factoryGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(false));
                    cameras[0].SetActive(true);
                    cameras[1].SetActive(false);
                }
            }
            forceOnForTestPlayer.ForEach(currentGameObject => currentGameObject.SetActive(true));
            return;
        }
        shopGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(false));
        factoryGameObjects.ForEach(currentGameObject => currentGameObject.SetActive(false));
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

    void Update() {  }

    public void EndGame()
    {
        EndTimers();
        MenuManager.instance.Open("GameOver");
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

    public void ShowBalanceInfo(string text, Color color)
    {
        GameObject textObject = new GameObject("text-" + text);

        textObject.transform.SetParent(balanceInfo.transform);
        textObject.transform.localPosition = Vector3.zero;
        textObject.transform.localScale = Vector3.one;
        textObject.transform.SetSiblingIndex(0);

        TMP_Text tmpText = textObject.AddComponent<TextMeshProUGUI>();

        tmpText.text = text;
        tmpText.color = color;
        tmpText.fontSize = 23;
        tmpText.fontStyle = FontStyles.Normal;
        tmpText.alignment = TextAlignmentOptions.Right;
        tmpText.font = fontAsset;


        new ActionTimer(() => Destroy(textObject), 3).Run();
    }
}
