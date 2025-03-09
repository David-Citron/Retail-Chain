using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class GoodsDelivery : Interactable
{
    public static GoodsDelivery instance;

    private List<DeliveryOffer> deliveryOffers;

    [SerializeField] private GameObject vehicle;
    [SerializeField] private GameObject garageDoor;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button departureButton;

    public GameObject itemPrefab;
    public GameObject itemListContent;

    private float elapsedTime;
    private bool isMoving;

    private const int TIME_BEFORE_DELIVERY = 5; //Every 35 seconds.

    void Start()
    {
        instance = this;
        deliveryOffers = new List<DeliveryOffer>();

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear && IsActive(), gameObject => ToggleOffersUI(), new Hint(() => Hint.GetHintButton(ActionType.PickUpItem) + " TO OPEN OFFERS", () => isPlayerNear && IsActive())));

        StartCoroutine(StartDeliveryTimer());

        closeButton.gameObject.SetActive(true);
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => ToggleOffersUI());

        departureButton.gameObject.SetActive(true);
        departureButton.onClick.RemoveAllListeners();
        departureButton.onClick.AddListener(() => ClearOffers());
    }

    void Update() {}

    void FixedUpdate()
    {
        if (!isMoving) return;
        PlayTruckAnimation(IsActive());
    }

    public void ToggleOffersUI()
    {
        if (!MenuManager.instance.ToggleUI("DeliveryOffers") || !IsActive()) return;
        UpdateOffers();
    }

    public void UpdateOffers()
    {
        foreach (var item in deliveryOffers)
        {
            if(item.contentItem != null) Destroy(item.contentItem);
            if (item.itemAmount <= 0) continue;
            
            GameObject createdItem = Instantiate(itemPrefab);

            createdItem.transform.SetParent(itemListContent.transform);
            createdItem.transform.localScale = Vector3.one;

            var offer = createdItem.GetComponent<DeliveryOfferItem>();
            offer.Initialize(item);

            item.contentItem = createdItem;
        }

        deliveryOffers.RemoveAll(deliveryOffer => deliveryOffer.itemAmount <= 0);

        if (IsActive()) return;
        Hint.Create("NO MORE OFFERS", 2);
        ClearOffers();
    }

    private IEnumerator StartDeliveryTimer()
    {
        yield return new WaitForSeconds(2f);
        new ActionTimer(() =>
        {
            GenerateOffers();
        }, TIME_BEFORE_DELIVERY).Run();
    }

    private void PlayTruckAnimation(bool inAnimation)
    {
        if (vehicle == null || elapsedTime >= 3.5f)
        {
            isMoving = false;
            garageDoor.gameObject.SetActive(!inAnimation);
            if (!inAnimation) vehicle.SetActive(false);
            return;
        }

        if (inAnimation && !vehicle.activeSelf) vehicle.SetActive(true);
        if(!inAnimation && !garageDoor.activeSelf) garageDoor.SetActive(true);

        elapsedTime += Time.fixedDeltaTime;
        float t = elapsedTime / 50f;

        float newZ = Mathf.Lerp(vehicle.transform.localPosition.z, inAnimation ? -5.8f : -8, t);
        vehicle.transform.localPosition = new Vector3(vehicle.transform.localPosition.x, vehicle.transform.localPosition.y, newZ);
    }


    private void GenerateOffers()
    {
        deliveryOffers.Clear();
        elapsedTime = 0f;
        isMoving = true;

        System.Random random = new System.Random();

        for (int i = 0; i < random.Next(2, 4); i++)
        {
            ItemData data = GetRandomType(random);
            deliveryOffers.Add(new DeliveryOffer(data, random.Next(data.buyPrice, TaxesManager.GetInflactionPrice(data.buyPrice)), random.Next(1, data.maxOfferAmount)));
        }
    }

    private void ClearOffers()
    {
        if (MenuManager.instance.IsOpened("DeliveryOffers")) ToggleOffersUI();

        foreach (var offer in deliveryOffers)
        {
            if (offer.contentItem == null) continue;
            Destroy(offer.contentItem);
        }

        deliveryOffers.Clear();
        elapsedTime = 0f;
        isMoving = true;

        new ActionTimer(() => StartCoroutine(StartDeliveryTimer()), TIME_BEFORE_DELIVERY).Run();
    }

    private ItemData GetRandomType(System.Random random)
    {
        List<ItemData> values = ItemManager.GetBuyableItemData();
        var value = values[random.Next(values.Count)];
        while (deliveryOffers.Any(item => item.item == value)) value = values[random.Next(values.Count)];
        return value;
    }

    private bool IsActive() => deliveryOffers.Count > 0;
    public override string GetTag() => "GoodsDelivery";
}

public class DeliveryOffer
{
    public ItemData item { get; private set; }
    public GameObject contentItem {  get; set; }

    public int price { get; private set; }
    public int itemAmount { get; set; }

    public DeliveryOffer(ItemData item, int price, int itemAmount)
    {
        this.item = item;
        this.price = price;
        this.itemAmount = itemAmount;
    }
}