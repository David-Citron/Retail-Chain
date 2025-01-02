using System.Collections.Generic;
using UnityEngine;

public class ItemStorage : Reachable
{

    private Dictionary<ItemType, int> storedItems = new Dictionary<ItemType, int>();

    void Start()
    {
        
    }

    void Update()
    {
        if (!isReachable) return;
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPickUp.Instance().IfPresent(pickUp =>
            {
                if (pickUp.holdingItem == null) return;
                InsertItem(pickUp.holdingItem);
            });

            return;
        }

        if (!Input.GetKeyDown(KeyCode.E)) return;
        OpenUI();
    }

    public void OpenUI()
    {

    }

    public void InsertItem(GameObject gameObject)
    {
        ItemType itemType = Item.GetItemType(gameObject).GetValueOrDefault();
        if(itemType == ItemType.None)
        {
            Hint.Create("INVALID ITEM", 2);
            return;
        }

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            pickUp.DropHoldingItem();
            Destroy(gameObject);
        });

        storedItems.Add(itemType, GetStoredAmountOf(itemType) + 1);
    }

    public void TakeItem(ItemType itemType)
    {
        int amount = GetStoredAmountOf(itemType);

        if (amount <= 0)
        {
            Hint.Create("THERE IS NOT ENOUGH ITEMS", 2);
            return;
        }

        storedItems.Add(itemType, amount - 1);
        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            GameObject item = Item.GetGameObjectFromPrefab(itemType);
            if (item == null) return;
            pickUp.PickUp(item);
        });
    }

    protected override void OnReachableChange()
    {

    }

    public int GetStoredAmountOf(ItemType itemType) => storedItems.GetValueOrDefault(itemType, 0);
}
