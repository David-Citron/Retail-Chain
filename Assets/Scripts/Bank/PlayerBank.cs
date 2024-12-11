using UnityEngine;

public class PlayerBank : MonoBehaviour
{

    private int balance;

    void Start() {}

    void Update() {}


    /// <summary>
    /// Adds balance to player's bank.
    /// </summary>
    /// <param name="amount">Amount to be added</param>
    public void AddBalance(int amount)
    {
        balance += amount;
        UpdateMenu();
    }

    /// <summary>
    /// Removes balance from player's bank.
    /// </summary>
    /// <param name="amount">Amount to be removed</param>
    /// <returns>true if it was successful, otherwise false</returns>
    public bool RemoveBalance(int amount)
    {
        if (balance < amount) return false;
        balance -= amount;
        UpdateMenu();
        return true;
    }

    public void CalculateCharges()
    {
        int charges = (int) (balance * 0.025m);


        if (!RemoveBalance(charges)) Game.Instance().IfPresent(game => game.setGameState(GameState.Ending));
    }

    public int GetBalance() => balance;

    private void UpdateMenu()
    {
        GameLayoutManager.Instance().IfPresent(layout => layout.UpdateBalance(balance));
    }
}
