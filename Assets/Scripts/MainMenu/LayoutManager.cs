using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LayoutManager : MonoBehaviour
{



    [SerializeField] public List<TMP_Text> userNames = new List<TMP_Text>();
    [SerializeField] public List<RawImage> profilePictures = new List<RawImage>();

    [SerializeField] public List<Button> readyButtons = new List<Button>();
    [SerializeField] public List<TMP_Text> readyTextButtons = new List<TMP_Text>();

    [SerializeField] public Button leaveButton;

    [SerializeField] private GameObject defaultButtonsGroup;
    [SerializeField] private GameObject playButtonsGroup;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject steamNotInitializedNotification;

    void Start()
    {
        defaultButtonsGroup.SetActive(true);
        playButtonsGroup.SetActive(false);
        lobby.SetActive(false);
    }

    public void ChangeActive(GameObject button)
    {
        button.SetActive(!button.activeInHierarchy);
    }

    public void BackToMainMenu(GameObject current)
    {
        defaultButtonsGroup.SetActive(true);
        current.SetActive(false);
    }

    public void ShowLobby()
    { 
        lobby.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        lobby.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void ShowSteamNotInitializedNotification()
    {
        steamNotInitializedNotification.SetActive(true);
    }
}
