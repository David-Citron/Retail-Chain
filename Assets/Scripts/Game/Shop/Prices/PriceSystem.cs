using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class PriceSystem : Interactable
{
    private static PriceSystem instance;

    private List<GameObject> priceItems;

    [SerializeField] private GameObject contentList;
    [SerializeField] private GameObject priceItemPrefab;
    [SerializeField] private Button closeButton;

    private bool isPlayerNear;

    void Start()
    {
        instance = this;
        priceItems = new List<GameObject>();


        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear, gameObject => UpdateContent(false),
            new Hint(Hint.GetHintButton(ActionType.PickUpItem) + " TO MANAGE PRICES", () => isPlayerNear)));

        closeButton.interactable = true;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => UpdateContent(false));

        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;

    }

    void Update() {}

    public static void UpdatePrice(ItemType itemType, int newPrice)
    {
        ItemManager.GetItemData(itemType).sellPrice = newPrice;
        instance.UpdateContent(true);
    }

    public void UpdateContent(bool update)
    {
        if (!update && !GameLayoutManager.instance.ToggleUI(LayoutType.PriceSystem)) return;
        priceItems.ForEach(item => Destroy(item));
        foreach (var item in ItemManager.GetAllSellableItemData())
        {
            GameObject createdItem = Instantiate(priceItemPrefab);

            createdItem.transform.SetParent(contentList.transform);
            createdItem.transform.localScale = Vector3.one;

            var offer = createdItem.GetComponent<PriceItem>();
            offer.Initialize(item);

            priceItems.Add(createdItem);
        }
    }

    public override string GetTag() => "CashierRegister";
    public override bool IsPlayerNear() => isPlayerNear;
    public override void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
}
