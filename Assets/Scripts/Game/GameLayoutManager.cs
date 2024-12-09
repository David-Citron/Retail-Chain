using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLayoutManager : MonoBehaviour
{

    private static GameLayoutManager instance;

    [SerializeField] private TMP_Text username;
    [SerializeField] private RawImage profilePicture;

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

    }

    public static Optional<GameLayoutManager> Instance() => instance == null ? Optional<GameLayoutManager>.Empty() : Optional<GameLayoutManager>.Of(instance);
}
