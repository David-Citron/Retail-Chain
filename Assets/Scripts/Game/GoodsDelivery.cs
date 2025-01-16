using System.Collections.Generic;
using UnityEngine;
using System;

public class NewBehaviourScript : MonoBehaviour
{

    private List<DeliveryOffer> deliveryOffers;

    private const int OFFERS = 3;
    private const int TIME_BEFORE_DELIVERY = 55; //Every 55 seconds.

    void Start()
    {
        deliveryOffers = new List<DeliveryOffer>();   
    }

    void Update()
    {
        
    }

    private void StartDeliveryTimer()
    {
        new ActionTimer(() =>
        {
            GenerateOffers();

            new ActionTimer(() => StartDeliveryTimer(), 20, 1).Run(); //After 20 seconds start timer for the new delivery.
        }, TIME_BEFORE_DELIVERY, 1).Run();
    }

    private void PlayTruckAnimation()
    {

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