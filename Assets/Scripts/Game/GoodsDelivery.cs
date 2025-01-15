using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    private List<DeliveryOffer> deliveryOffers;

    private const int TIME_BEFORE_DELIVERY = 55; //Every 55 seconds.

    void Start()
    {
        deliveryOffers = new List<DeliveryOffer>();   
    }

    void Update()
    {
        
    }


    private void GenerateOffers()
    {
        //Some logic based on the contract? Or just 3 random items - discuss.
    }


    private void StartDeliveryTimer()
    {
        new ActionTimer(() =>
        {
        }, TIME_BEFORE_DELIVERY, 1).Run();
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