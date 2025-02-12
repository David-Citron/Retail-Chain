using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PriceSystem : MonoBehaviour
{
    private static PriceSystem instance;

    private List<GameObject> priceItems;
    [SerializeField] private GameObject contentList;
    [SerializeField] private GameObject priceItemPrefab;
    [SerializeField] private Button closeButton;

    void Start()
    {
        instance = this;

        priceItems = new List<GameObject>();

        Interactable.AddInteraction(new Interaction(() => Interactable.PressedKey(ActionType.PickUpItem), gameObject => UpdateContent()));

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => UpdateContent());
    }

    void Update() {}

    public static void UpdatePrice(ItemType itemType, int newPrice)
    {
        ItemManager.GetItemData(itemType).sellPrice = newPrice;
        instance.UpdateContent();
    }

    public void UpdateContent()
    {
        if (!GameLayoutManager.instance.ToggleUI(LayoutType.PriceSystem)) return;
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
}
