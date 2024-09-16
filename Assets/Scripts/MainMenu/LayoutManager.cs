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

    [SerializeField] public List<TMP_Text> roleTexts = new List<TMP_Text>();

    [SerializeField] public Button leaveButton;
    [SerializeField] public Button swapButton;

    [SerializeField] private GameObject defaultButtonsGroup;
    [SerializeField] private GameObject playButtonsGroup;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject steamNotInitializedNotification;

    [SerializeField] private TMP_Text notificationText;

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

    public void SendNotification(string text, int time)
    {
        SendColoredNotification(text, Color.white, time);
    }

    public void SendColoredNotification(string text, Color color, int time)
    {
        StartCoroutine(SendEnumaratorNotification(text, color, time));
    }

    public IEnumerator SendEnumaratorNotification(string text, Color color, int time)
    {
        Debug.Log("New notification: " + text + " for " + time);
        notificationText.text = text;
        notificationText.color = color;
        yield return new WaitForSecondsRealtime(time);
        notificationText.text = "";
        notificationText.color = Color.white;
    }
}
