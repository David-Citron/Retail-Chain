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

        username.text = PlayerSteamUtils.GetSteamUsername(SteamUser.GetSteamID());
        profilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(SteamUser.GetSteamID());
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Activates UI based on the given layout type.
    /// </summary>
    /// <param name="layoutType"></param>
    /// <returns>returns if the ui is enabled or disabled</returns>
    public bool ToggleUI(LayoutType layoutType)
    {
        if (IsEnabled(LayoutType.GameOver)) return false;

        GameObject gameObject = layouts[(int)layoutType];
        if (gameObject == null) return false;

        gameObject.SetActive(!gameObject.activeSelf);
        background.SetActive(gameObject.activeSelf);

        playerInfoField.SetActive(!background.activeSelf);
        isOpened = gameObject.activeSelf;
        return gameObject.activeSelf;
    }

    public bool IsEnabled(LayoutType layoutType) => layouts[(int)layoutType] != null && layouts[(int)layoutType].activeSelf;

    /// <summary>
    /// Closes any opened UIs except Contract.
    /// </summary>
    /// <returns>true if any UI was closed, otherwise false</returns>
    public bool CloseOpenedUI()
    {
        bool closedAny = false;
        foreach (LayoutType layouType in Enum.GetValues(typeof(LayoutType)))
        {
            if (layouType == LayoutType.Contract || layouType == LayoutType.GameOver) continue;
            if (!IsEnabled(layouType)) continue;
            ToggleUI(layouType);
            closedAny = true;
        }

        return closedAny;
    }

    public void UpdateBalance(int amount) => balance.text = "$" + amount.ToString("N0", CultureInfo.InvariantCulture);
    public void UpdateBalance() => UpdateBalance(PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault().bankAccount.GetBalance());
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
