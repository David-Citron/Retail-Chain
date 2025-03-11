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
    /// <param name="text">Text that will show to the player as a reason</param>
    public void AddBalance(int amount, string text)
    {
        balance += amount;
        income += amount;
        UpdateMenu();
        ShowInfo(amount, text);
    }

    public void AddBalance(int amount) => AddBalance(amount, null);

    /// <summary>
    /// Removes balance from player's bank.
    /// </summary>
    /// <param name="amount">Amount to be removed</param>
    /// <param name="text">Text that will show to the player as a reason</param>
    /// <returns>true if it was successful, otherwise false</returns>
    public bool RemoveBalance(int amount, string text)
    {
        if (balance < amount) return false;
        balance -= amount;
        UpdateMenu();
        ShowInfo(-amount, text);
        return true;
    }

    public bool RemoveBalance(int amount) => RemoveBalance(amount, null);

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
        if (RemoveBalance(amount, "Taxes")) return;

        //End game
        Game.instance.EndGame();
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

    /// <summary>
    /// Text is displayed below the money balance to inform the player of their income and costs.
    /// </summary>
    public void ShowInfo(int amount, string text)
    {
        if (Game.instance == null) return;

        string displayText = (amount > 0 ? "+" : "") + amount + "$";
        if(text != null) displayText += " [" + text + "]";

        Game.instance.ShowBalanceInfo(displayText, amount < 0 ? Color.red : Color.green);
    }
}
