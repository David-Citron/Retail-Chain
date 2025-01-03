using Edgegap;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class ItemStorage : Reachable
{

    public static ItemStorage instance;

    private Dictionary<ItemType, int> storedItems = new Dictionary<ItemType, int>();

    public GameObject storageUi;
    public GameObject itemPrefab;
    public GameObject itemListContent;

    void Start() { instance = this; }

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
        ToggleUI();
    }

    public void ToggleUI()
    {
        storageUi.SetActive(!storageUi.activeSelf);


        if (!storageUi.activeSelf) return;

        foreach (var item in storedItems.Keys)
        {
            GameObject createdItem = Instantiate(itemPrefab);

            var component = createdItem.GetComponent<StorageContentItem>();

            component.Initialize(item, storedItems[item]);

            createdItem.transform.SetParent(itemListContent.transform);
            createdItem.transform.localScale = Vector3.one;
        }

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


        ShowHints();

        if (storedItems.ContainsKey(itemType))
        {
            storedItems[itemType] = GetStoredAmountOf(itemType) + 1;
            return;
        }

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

        if (amount > 1) storedItems[itemType] = amount - 1;
        else storedItems.Remove(itemType);

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            GameObject item = Item.GetGameObjectFromPrefab(itemType);
            if (item == null)
            {
                Debug.LogWarning("error");
                return;
            }
            Debug.LogWarning("picked up");
            pickUp.PickUp(item);
            ToggleUI(); //Close.
            ShowHints();
        });
    }

    protected override void OnReachableChange()
    {
        ShowHints();
    }

    private void ShowHints()
    {
        if (!isReachable) return;
        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            if (pickUp.holdingItem == null) Hint.ShowWhile(HintText.GetHintButton(HintButton.E) + " TO PICK UP", () => isReachable && pickUp.holdingItem == null);
            else Hint.ShowWhile(HintText.GetHintButton(HintButton.SPACE) + " TO INSERT", () => isReachable && pickUp.holdingItem != null);
        });
    }

    public int GetStoredAmountOf(ItemType itemType) => storedItems.GetValueOrDefault(itemType, 0);
}
