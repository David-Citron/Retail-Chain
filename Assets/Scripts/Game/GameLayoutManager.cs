using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLayoutManager : MonoBehaviour
{

    public static GameLayoutManager instance;


    [SerializeField] private GameObject background;
    [SerializeField] private List<GameObject> layouts;

    [SerializeField] private GameObject playerInfoField;
    [SerializeField] private TMP_Text username;
    [SerializeField] private RawImage profilePicture;


    [SerializeField] private TMP_Text balance;

    void Start()
    {
        instance = this;

        UpdatePlayer();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Activates UI based on the given layout type, returns if the ui is enabled or disabled.
    /// </summary>
    /// <param name="layoutType"></param>
    /// <returns></returns>
    public bool ToggleUI(LayoutType layoutType)
    {
        GameObject gameObject = layouts[(int)layoutType];
        if (gameObject == null) return false;

        background.SetActive(!background.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);

        playerInfoField.SetActive(!background.activeSelf);

        return gameObject.activeSelf;
    }

    public bool IsEnabled(LayoutType layoutType) => layouts[(int)layoutType] != null && layouts[(int)layoutType].activeSelf;

    private void UpdatePlayer()
    {
        username.text = PlayerSteamUtils.GetSteamUsername(SteamUser.GetSteamID());
        profilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(SteamUser.GetSteamID());
    }

    public void UpdateBalance(int amount) => balance.text = amount.ToString();
}

public enum LayoutType
{
    ItemRack,

}
