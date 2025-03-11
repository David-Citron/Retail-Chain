using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Org.BouncyCastle.Math.EC.Multiplier;

[RequireComponent(typeof(BoxCollider))]
public class PriceSystem : Interactable
{
    public static PriceSystem instance;

    private List<GameObject> priceItems;

    private Dictionary<ItemType, int> itemPrices;

    [SerializeField] private GameObject contentList;
    [SerializeField] private GameObject priceItemPrefab;
    [SerializeField] private Button closeButton;

    private ActionTimer timer;

    void Start()
    {
        instance = this;
        priceItems = new List<GameObject>();
        itemPrices = new Dictionary<ItemType, int>();

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear, gameObject => UpdateContent(false),
            new Hint(() => Hint.GetHintButton(ActionType.PickUpItem) + " TO MANAGE PRICES", () => isPlayerNear)));

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && CustomerManager.instance.GetCustomerAtCashRegister() != null && isPlayerNear, i => ProcessPayment(),
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO PROCESS PAYMENT", () => CustomerManager.instance.GetCustomerAtCashRegister() != null && isPlayerNear)));

        closeButton.interactable = true;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => UpdateContent(false));

        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;


        ItemManager.GetAllSellableItemData().ForEach(item => itemPrices.Add(item.itemType, item.sellPrice));
    }

    void Update() {}

    private void ProcessPayment()
    {
        if (timer != null) return;
        var customer = CustomerManager.instance.GetCustomerAtCashRegister();
        if (customer == null) return;

        PlayerInputManager.isInteracting = true;

        CircleTimer.Start(3);

        timer = new ActionTimer(() => HoldingKey(ActionType.Interaction), 
            () => {
            PlayerInputManager.isInteracting = false;
            customer.Pay();
                PlayerPickUp.Instance().IfPresent(pickUp => pickUp.animator.SetTrigger("customer_wave"));
            timer = null;
            },
        () => {
            PlayerInputManager.isInteracting = false;
            CircleTimer.Stop();
            timer = null;
        }, 3, 1).Run();
    }

    public static int GetPrice(ItemType itemType) => instance.itemPrices[itemType];
    public static void UpdatePrice(ItemType itemType, int newPrice)
    {
        instance.itemPrices[itemType] = newPrice;
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

    /// <summary>
    /// Calculates the max price based on the inflation & shop rating.
    /// Rating (max 5):
    /// 1: -10%
    /// 2: -5%
    /// 3: 0%
    /// 4: +2%
    /// 5: +5%
    /// </summary>
    /// <param name="basePrice">The base price</param>
    /// <returns>The max price</returns>
    public static int CalculateMaxPrice(int basePrice)
    {
        float rating = ShopRating.GetRating();
        int newPrice = TaxesManager.GetInflationPrice(basePrice);

        float modifier = 0;

        switch (rating)
        {
            case 0:
                modifier = -0.15f;
                break;
            case 1:
                modifier = -0.10f;
                break;
            case 2:
                modifier = -0.05f;
                break;
            case 3:
                modifier =  0;
                break;
            case 4:
                modifier =  0.02f;
                break;
            case 5:
                modifier =  0.05f;
                break;
            default:
                modifier = 0;
                break;
        }

        newPrice = (int) (newPrice * (1 + modifier));
        return newPrice;
    }

    /// <summary>
    /// Recommended price will be shown to player and is lowered by 5%.
    /// </summary>
    /// <param name="basePrice">The base price</param>
    /// <returns>The max price reduced by 5%</returns>
    public static int CalculateRecommendedPrice(int basePrice) => (int) (CalculateMaxPrice(basePrice) * 0.95d);

    public override string GetTag() => "CashierRegister";
}
