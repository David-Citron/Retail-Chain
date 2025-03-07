using Edgegap;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInitializer : MonoBehaviour
{


    //Main Menu
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private RawImage mainMenuProfilePicture;
    [SerializeField] private TMP_Text mainMenuUsername;

    //Lobby Menu
    [SerializeField] public Button swapButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] public Button readyButton;
    [SerializeField] public Button readyCancelButton;

    [SerializeField] private Button lobbyType;
    [SerializeField] private Button inviteFriend;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
