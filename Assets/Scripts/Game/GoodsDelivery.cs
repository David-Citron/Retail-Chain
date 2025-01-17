using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{

    private List<DeliveryOffer> deliveryOffers;

    [SerializeField] private GameObject truck;
    [SerializeField] private GameObject garageDoor;

    private float elapsedTime;
    private bool isMoving;

    private const int OFFERS = 3;
    private const int TIME_BEFORE_DELIVERY = 5; //Every 55 seconds.

    void Start()
    {
        deliveryOffers = new List<DeliveryOffer>();
        StartCoroutine(StartDeliveryTimer());
    }

    void Update()
    {
        if (!isMoving) return;
        PlayTruckAnimation(deliveryOffers.Count > 0);
    }

    private IEnumerator StartDeliveryTimer()
    {
        yield return new WaitForSeconds(2f);
        new ActionTimer(() =>
        {
            garageDoor.gameObject.SetActive(false);
            GenerateOffers();

            //new ActionTimer(() => StartDeliveryTimer(), 20, 1).Run(); //After 20 seconds start timer for the new delivery.
        }, TIME_BEFORE_DELIVERY, 1).Run();
    }

    private void PlayTruckAnimation(bool inAnimation)
    {
        if (truck == null || elapsedTime >= 3f)
        {
            isMoving = false;
            return;
        }

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / 150f;

        float newZ = Mathf.Lerp(truck.transform.localPosition.z, inAnimation ? -6 : -8, t);
        truck.transform.localPosition = new Vector3(truck.transform.localPosition.x, truck.transform.localPosition.y, newZ);
    }


    private void GenerateOffers()
    {
        //Some logic based on the contract? Or just 3 random items - discuss.

        deliveryOffers.Clear();

        System.Random random = new System.Random();
        for(int i = 0; i < OFFERS; i++)
        {
            deliveryOffers.Add(new DeliveryOffer(GetRandomType(random), random.Next(50, 150)));
        }

        elapsedTime = 0f;
        isMoving = true;
    }

    private ItemType GetRandomType(System.Random random)
    {
        Array values = Enum.GetValues(typeof(ItemType));
        return (ItemType) values.GetValue(random.Next(values.Length));
    }
}

public class DeliveryOffer
{
    public ItemType item { get; private set; }
    public int price { get; private set; }

    public DeliveryOffer(ItemType item, int price)
    {
        this.item = item;
        this.price = price;
    }
}