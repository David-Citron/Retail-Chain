using UnityEngine;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{

    public Button backButton;
    public Button settingsButton;
    public Button exitButton;

    void Start()
    {
        Initialize();
        Interactable.AddInteraction(new Interaction(() => Interactable.PressedKey(ActionType.OpenMenu), pressedTime =>
        GameLayoutManager.instance.ToggleUI(LayoutType.Exit)));
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

        backButton.onClick.AddListener(() => GameLayoutManager.instance.ToggleUI(LayoutType.Exit));


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
