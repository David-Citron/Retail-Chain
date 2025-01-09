using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }


   private void GenerateOffers()
    {
        //Some logic based on the contract? Or just 3 random items - discuss.
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