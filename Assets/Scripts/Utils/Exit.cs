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
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        exitUI.SetActive(!exitUI.activeSelf);
    }


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
            SteamLobby.instance.LeaveLobby();
        });
    }
}
