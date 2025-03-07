using UnityEngine;

public class PlayerBank : MonoBehaviour
{
    private int balance;
    private int income; //This is income before next contract, after that it will be reset to 0.

    void Start() 
    {
        balance = 1000;
        UpdateMenu();
    }

    void Update() {}

    /// <summary>
    /// Adds balance to player's bank.
    /// </summary>
    /// <param name="amount">Amount to be added</param>
    public void AddBalance(int amount)
    {
        balance += amount;
        income += amount;
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

    /// <summary>
    /// This method pays all player's taxes.
    /// </summary>
    public void PayTaxes()
    {
        PayTax((int) (income * 0.1m)); //Income tax
        PayTax(TaxesManager.GetRentTaxes()); //Rent taxes (electricity, gas, rent)

        income = 0;
    }

    /// <summary>
    /// Pay tax, if player does not have enough money the game ends.
    /// </summary>
    /// <param name="amount">The tax charge</param>
    private void PayTax(int amount)
    {
        if (RemoveBalance(amount)) return;

        //End game
    }

    /// <summary>
    /// Gets the player's balance
    /// </summary>
    /// <returns>The balance</returns>
    public int GetBalance() => balance;

    private void UpdateMenu()
    {
        if (Game.instance == null) return;
        Game.instance.UpdateBalance(GetBalance());
    }
}
