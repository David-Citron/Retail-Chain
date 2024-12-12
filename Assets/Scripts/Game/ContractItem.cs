public class ContractItem
{
    public ItemType itemType;
    public int quantity;
    public int quantityRemaining { private set; get; }
    public bool fulfilled { private set; get; }

    /// <summary>
    /// Creates ContractItem.
    /// </summary>
    /// <param name="itemType">Sets type of the item.</param>
    /// <param name="quantity">Sets quantity of this item for the contract</param>
    public ContractItem(ItemType itemType, int quantity)
    {
        this.itemType = itemType;
        this.quantity = quantity;
        quantityRemaining = quantity;
        fulfilled = false;
    }

    /// <summary>
    /// One instance of an item was provided for the contract.
    /// </summary>
    public void ItemSubmitted()
    {
        if (quantityRemaining > 0) return;
        fulfilled = true;
    }
}
