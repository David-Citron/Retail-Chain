using UnityEngine;
using UnityEngine.UI;

public class Account : MonoBehaviour
{

    private ulong steamId;
    [SerializeField] private string username;
    private RawImage profilePicture;

    private int accountLevel;
    private int experience;

    public Account SetAccount(ulong steamId, string username, RawImage profilePicture, int accountLevel, int experience)
    {
        this.steamId = steamId;
        this.username = username;
        this.profilePicture = profilePicture;
        this.accountLevel = accountLevel;
        this.experience = experience;

        return this;
    }

    public ulong GetSteamId()
    {
        return steamId;
    }

    public string GetUsername()
    {
        return username;
    }

    public RawImage GetProfilePicture()
    {
        return profilePicture;
    }

    public int GetAccountLevel()
    {
        return accountLevel;
    }

    public int GetExperience()
    {
        return experience;
    }

    //TODO upravit max
    public void addExperience(int experience)
    {
        this.experience += experience;

    }

}