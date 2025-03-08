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
    private ActionTimer timer;

    void Start()
    {
        instance = this;
        priceItems = new List<GameObject>();


        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear, gameObject => UpdateContent(false),
            new Hint(Hint.GetHintButton(ActionType.PickUpItem) + " TO MANAGE PRICES", () => isPlayerNear)));


        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && CustomerManager.instance.FirstCustomerInQueue() != null && isPlayerNear, i => ProcessPayment(),
            new Hint(Hint.GetHintButton(ActionType.Interaction) + " TO PROCESS PAYMENT", () => CustomerManager.instance.FirstCustomerInQueue() != null && isPlayerNear)));

        closeButton.interactable = true;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => UpdateContent(false));

        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;

    }

    void Update() {}

    private void ProcessPayment()
    {
        if (timer != null) return;
        var customer = CustomerManager.instance.FirstCustomerInQueue();
        if (customer == null) return;

        PlayerInputManager.isInteracting = true;

        CircleTimer.Start(3);

        timer = new ActionTimer(() => {
            PlayerInputManager.isInteracting = false;
            customer.Pay();
            },
        () => {
            PlayerInputManager.isInteracting = false;
            CircleTimer.Stop();
            timer = null;
        }, 3).Run();
    }

    public static void UpdatePrice(ItemType itemType, int newPrice)
    {
        ItemManager.GetItemData(itemType).sellPrice = newPrice;
        instance.UpdateContent(true);
    }

    public void UpdateContent(bool update)
    {
        if (!update && !MenuManager.instance.ToggleUI("PriceSystem")) return;
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
