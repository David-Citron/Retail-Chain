using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class GooodsDelivery : Interactable
{

    private List<DeliveryOffer> deliveryOffers;
    private List<GameObject> contentItems = new List<GameObject>();

    [SerializeField] private GameObject truck;
    [SerializeField] private GameObject garageDoor;


    public GameObject itemPrefab;
    public GameObject itemListContent;

    private float elapsedTime;
    private bool isMoving;
    private bool isPlayerNear;

    private const int OFFERS = 3;
    private const int TIME_BEFORE_DELIVERY = 5; //Every 55 seconds.

    void Start()
    {
        deliveryOffers = new List<DeliveryOffer>();

        AddInteraction(new Interaction(GetTag(), () => Input.GetKeyDown(KeyCode.E) && isPlayerNear, gameObject => OpenOffers(), new Hint(Hint.GetHintButton(HintButton.E) + " TO OPEN OFFERS", () => isPlayerNear)));

        StartCoroutine(StartDeliveryTimer());
    }

    void Update() {}

    void FixedUpdate()
    {
        if (!isMoving) return;
        PlayTruckAnimation(deliveryOffers.Count > 0);
    }

    private void OpenOffers()
    {
        if (!GameLayoutManager.instance.ToggleUI(LayoutType.DeliveryOffers)) return;

        foreach (var item in contentItems) Destroy(item);
        foreach (var item in deliveryOffers)
        {
            GameObject createdItem = Instantiate(itemPrefab);

            createdItem.transform.SetParent(itemListContent.transform);
            createdItem.transform.localScale = Vector3.one;

            var offer = createdItem.GetComponent<DeliveryOffer>();
            offer.Initialize(item.);

            contentItems.Add(createdItem);
        }
    }

    private IEnumerator StartDeliveryTimer()
    {
        yield return new WaitForSeconds(2f);
        new ActionTimer(() =>
        {
            garageDoor.gameObject.SetActive(false);
            GenerateOffers();

            new ActionTimer(() => StartDeliveryTimer(), 20, 1).Run(); //After 20 seconds start timer for the new delivery.
        }, TIME_BEFORE_DELIVERY, 1).Run();
    }

    private void PlayTruckAnimation(bool inAnimation)
    {
        if (truck == null || elapsedTime >= 3.5f)
        {
            isMoving = false;
            return;
        }

        elapsedTime += Time.fixedDeltaTime;
        float t = elapsedTime / 50f;

        float newZ = Mathf.Lerp(truck.transform.localPosition.z, inAnimation ? -5.8f : -8, t);
        truck.transform.localPosition = new Vector3(truck.transform.localPosition.x, truck.transform.localPosition.y, newZ);
    }


    private void GenerateOffers()
    {
        //Some logic based on the contract? Or just 3 random items - discuss.

        deliveryOffers.Clear();

        System.Random random = new System.Random();
        for(int i = 0; i < OFFERS; i++)
        {
            deliveryOffers.Add(new DeliveryOffer(GetRandomType(random), random.Next(50, 150), random.Next(3, 6)));
        }

        elapsedTime = 0f;
        isMoving = true;
    }

    private ItemType GetRandomType(System.Random random)
    {
        Array values = Enum.GetValues(typeof(ItemType));
        return (ItemType) values.GetValue(random.Next(values.Length));
    }

    public override string GetTag() => "GoodsDelivery";
    public override bool IsPlayerNear() => isPlayerNear;
    public override void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
}

public class DeliveryOffer
{
    public ItemType item { get; private set; }
    public int price { get; private set; }
    public int itemAmount { get; private set; }

    public DeliveryOffer(ItemType item, int price, int itemAmount)
    {
        this.item = item;
        this.price = price;
        this.itemAmount = itemAmount;
    }
}