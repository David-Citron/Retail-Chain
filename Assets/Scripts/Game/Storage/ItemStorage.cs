using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStorage : Interactable
{

    public static ItemStorage instance;

    private Dictionary<ItemType, int> storedItems = new Dictionary<ItemType, int>();
    private List<GameObject> items = new List<GameObject>();

    public GameObject storageUi;
    public GameObject itemPrefab;
    public GameObject itemListContent;

    public Button closeButton;

    void Start() {
        instance = this;

        closeButton.interactable = true;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            ToggleUI();
        });

        AddInteraction(new Interaction(KeyCode.Space, collider => InsertItem(PlayerPickUp.holdingItem), new Hint[] {
            new Hint(HintText.GetHintButton(HintButton.SPACE) + " TO INSERT", () => PlayerPickUp.holdingItem != null)
        }));

        AddInteraction(new Interaction(KeyCode.E, collider => ToggleUI(), new Hint[] {
            new Hint(HintText.GetHintButton(HintButton.E) + " TO OPEN STORAGE", () => PlayerPickUp.holdingItem == null)
        }));
    }

    void Update() { }

    public void ToggleUI()
    {

        if(storedItems.Count <= 0 && !storageUi.activeSelf)
        {
            Hint.Create("NO ITEMS IN STORAGE", 1);
            return;
        }

        storageUi.SetActive(!storageUi.activeSelf);

        if (!storageUi.activeSelf) return;

        foreach (var item in items) Destroy(item);
        foreach (var item in storedItems.Keys)
        {
            GameObject createdItem = Instantiate(itemPrefab);

            var component = createdItem.GetComponent<StorageContentItem>();

            component.Initialize(item, storedItems[item]);

            createdItem.transform.SetParent(itemListContent.transform);
            createdItem.transform.localScale = Vector3.one;

            items.Add(createdItem);
        }
    }

    public void InsertItem(GameObject gameObject)
    {
        ItemType itemType = Item.GetItemType(gameObject).GetValueOrDefault();
        if(itemType == ItemType.None)
        {
            Hint.Create("INVALID ITEM", 1);
            return;
        }

        PlayerPickUp.Instance().IfPresent(pickUp =>
        {
            pickUp.DropHoldingItem();
            Destroy(gameObject);
        });

        UpdateHints();

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
            if (item == null) return;
            pickUp.PickUp(item);
            ToggleUI(); //Close.
            UpdateHints();
        });
    }

    public int GetStoredAmountOf(ItemType itemType) => storedItems.GetValueOrDefault(itemType, 0);
    public override string GetTag() => "ItemStorage";
}
