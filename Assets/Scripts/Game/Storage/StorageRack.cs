using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class StorageRack : Interactable
{
    public static StorageRack instance;

    private Dictionary<ItemType, int> storedItems;
    [SerializeField] private List<GameObject> rackItems = new List<GameObject>();
    [SerializeField] private List<ItemType> defaultItems = new List<ItemType>();
    private List<GameObject> contentItems;

    public GameObject itemPrefab;
    public GameObject itemListContent;

    public Button closeButton;

    void Start() {
        instance = this;

        storedItems = new Dictionary<ItemType, int>();
        contentItems = new List<GameObject>();

        foreach (var item in defaultItems)
        {
            InsertItem(item, 1);
        }

        GetComponent<Rigidbody>().isKinematic = true;

        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(80f, 50f, collider.size.z);

        closeButton.interactable = true;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => ToggleUI());

        UpdateRackItems();

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear, collider => InsertGameObject(PlayerPickUp.holdingItem), new Hint[] {
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO INSERT", () => PlayerPickUp.IsHodlingItem() && isPlayerNear)
        }));

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear, collider => ToggleUI(), new Hint[] {
            new Hint(() => Hint.GetHintButton(ActionType.PickUpItem) + " TO OPEN STORAGE", () => !PlayerPickUp.IsHodlingItem() && isPlayerNear)
        }));
    }

    void Update() {}

    public void ToggleUI()
    {
        if (PlayerPickUp.holdingItem != null) return;

        if(storedItems.Count <= 0 && !MenuManager.instance.IsOpened("StorageRack"))
        {
            Hint.ShowWhile(() => "NO ITEMS IN STORAGE", () => storedItems.Count <= 0 && isPlayerNear);
            return;
        }

        if (!MenuManager.instance.ToggleUI("StorageRack")) return;

        foreach (var item in contentItems) Destroy(item);
        foreach (var item in storedItems.Keys)
        {
            GameObject createdItem = Instantiate(itemPrefab);

            createdItem.transform.SetParent(itemListContent.transform);
            createdItem.transform.localScale = Vector3.one;

            var component = createdItem.GetComponent<StorageContentItem>();

            component.Initialize(item, storedItems[item]);

            contentItems.Add(createdItem);
        }
    }

    public void InsertItem(ItemType itemType, int amount)
    {
        if (itemType == ItemType.None)
        {
            Hint.Create("INVALID ITEM", 1);
            return;
        }

        UpdateHints();
        if (storedItems.ContainsKey(itemType))
        {
            storedItems[itemType] = GetStoredAmountOf(itemType) + amount;
            UpdateRackItems();
            return;
        }

        storedItems.Add(itemType, amount);
        UpdateRackItems();
    }

    public void InsertGameObject(GameObject gameObject)
    {
        ItemType itemType = ItemManager.GetItemType(gameObject).GetValueOrDefault();
        Item data = ItemManager.GetItemInfo(gameObject);

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            pickUp.DropHoldingItem();
            Destroy(gameObject);
        });

        if(data != null && data.contentType != ItemType.None)
        {
            Contract localContract = ContractManager.instance.localContract;
            if (localContract == null || !localContract.SubmitItem(data.contentType))
            {
                InsertItem(ItemType.Package, 1);
                InsertItem(data.contentType, 1);
                return;
            }
            return;
        }

        InsertItem(itemType, 1);
    }

    public void TakeItem(ItemType itemType, bool validate)
    {
        int amount = GetStoredAmountOf(itemType);

        if (amount <= 0 && validate)
        {
            Hint.Create("THERE IS NOT ENOUGH ITEMS", 2);
            return;
        }
        
        if (validate) {
            if (amount > 1) storedItems[itemType] = amount - 1;
            else storedItems.Remove(itemType);
        }

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            GameObject item = ItemManager.CreateItem(itemType);
            if (item == null) return;

            if (validate) ToggleUI();

            pickUp.PickUp(item);
            UpdateHints();
            UpdateRackItems();
        });
    }

    private void UpdateRackItems()
    {
        int totalItemsAmount = GetStoredAmount();
        int activeItemCount = Mathf.Clamp(totalItemsAmount / 5, 0, rackItems.Count);

        for (int i = 0; i < rackItems.Count; i++)
        {
            rackItems[i].SetActive(totalItemsAmount != 0 && i <= activeItemCount);
        }


        if (totalItemsAmount == 1) rackItems[0].SetActive(true); // If there at least one enable first item for better visual effect.
    }


    private int GetStoredAmount()
    {
        int amount = 0;
        foreach(int number in storedItems.Values)
        {
            amount += number;
        }
        return amount;
    }


    public int GetStoredAmountOf(ItemType itemType) => storedItems.GetValueOrDefault(itemType, 0);
    public override string GetTag() => "StorageRack";
}
