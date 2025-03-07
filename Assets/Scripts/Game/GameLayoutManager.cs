using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System;

public class GameLayoutManager : MonoBehaviour
{

    public static GameLayoutManager instance;
    public static bool isOpened; //If any UI is opened it is true, otherwise its false.

    [SerializeField] private GameObject background;
    [SerializeField] private List<GameObject> layouts;

    [SerializeField] private GameObject playerInfoField;
    [SerializeField] private TMP_Text username;
    [SerializeField] private RawImage profilePicture;

    [SerializeField] private TMP_Text balance;

    void Start()
    {
        instance = this;
        isOpened = false;

        username.text = PlayerSteamUtils.GetSteamUsername(SteamUser.GetSteamID());
        profilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(SteamUser.GetSteamID());
    }

    void Update()
    {
        
    }


    public void UpdateBalance(int amount) => balance.text = "$" + amount.ToString("N0", CultureInfo.InvariantCulture);
}

public enum LayoutType
{
    ItemRack,
    DeliveryOffers,
    Contract,
    PriceSystem,
    GameOver,
    Exit
}
