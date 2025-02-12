using UnityEngine;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{

    public GameObject exitUI;

    public Button backButton;
    public Button settingsButton;
    public Button exitButton;

    void Start()
    {
        Initialize();
        Interactable.AddInteraction(new Interaction(() => Interactable.PressedKey(ActionType.OpenMenu), pressedTime =>
        exitUI.SetActive(!exitUI.activeSelf)));
    }

    void Update() {}


    public void Initialize()
    {
        backButton.interactable = true;
        settingsButton.interactable = true;
        exitButton.interactable = true;

        backButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();

        backButton.onClick.AddListener(() =>
        {
            exitUI.SetActive(!exitUI.activeSelf);
        });


        exitButton.onClick.AddListener(() =>
        {
            PlayerManager.instance.GetLocalGamePlayer().IfPresent(gamePlayer =>
            {
                if (gamePlayer.isServer && gamePlayer.isClient) CustomNetworkManager.singleton.StopHost();
                else CustomNetworkManager.singleton.StopClient();
            });
        });
    }
}
