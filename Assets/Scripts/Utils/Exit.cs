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
        Interactable.AddInteraction(new Interaction(() => Interactable.PressedKey(ActionType.OpenMenu, true), item => {
            if (MenuManager.instance.CloseCurrent()) return;
                
            MenuManager.instance.ToggleUI("Exit");
        }));

        Interactable.AddInteraction(new Interaction(() => Interactable.PressedKey(ActionType.Help), item =>
        {
            PlayerManager.instance.GetLocalGamePlayer().IfPresent(player =>
            {
                if (player.playerRole == PlayerRole.Factory) MenuManager.instance.ToggleUI("FactoryHelp");
                else MenuManager.instance.ToggleUI("ShopHelp");
            });
        }));
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

        backButton.onClick.AddListener(() => MenuManager.instance.ToggleUI("Exit"));


        exitButton.onClick.AddListener(() => Leave());
    }

    public static void Leave()
    {
        if (CustomNetworkManager.instance == null) return;
        if (PlayerManager.instance == null) return;
        GamePlayer localPlayer = PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault();
        if (localPlayer.isClientOnly) CustomNetworkManager.instance.StopClient();
        else if (localPlayer.isClient) CustomNetworkManager.instance.StopHost();
    }
}
