[System.Serializable]
public class ContractItem
{
    public ItemType itemType;
    public int quantity;
    public int quantityRemaining;
    public bool fulfilled;
    public int price;

    /// <summary>
    /// Creates ContractItem.
    /// </summary>
    /// <param name="itemType">Sets type of the item.</param>
    /// <param name="quantity">Sets quantity of this item for the contract</param>
    public ContractItem(ItemType itemType, int quantity, int price)
    {
        this.itemType = itemType;
        this.quantity = quantity;
        this.price = price;
        quantityRemaining = quantity;
        fulfilled = false;
    }

    public ContractItem() { }

    /// <summary>
    /// One instance of an item was provided for the contract.
    /// </summary>
    public void ItemSubmitted()
    {
        quantityRemaining--;
        if (quantityRemaining > 0) return;
        fulfilled = true;
    }
}
