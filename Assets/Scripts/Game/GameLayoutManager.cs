using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLayoutManager : MonoBehaviour
{

    private static GameLayoutManager instance;

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


    private void UpdatePlayer()
    {
        username.text = PlayerSteamUtils.GetSteamUsername(SteamUser.GetSteamID());
        profilePicture.texture = PlayerSteamUtils.GetSteamProfilePicture(SteamUser.GetSteamID());
    }

    public void UpdateBalance(int amount)
    {
        balance.text = amount.ToString();
    }

    public static Optional<GameLayoutManager> Instance() => instance == null ? Optional<GameLayoutManager>.Empty() : Optional<GameLayoutManager>.Of(instance);
}
