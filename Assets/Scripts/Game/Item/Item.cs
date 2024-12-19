using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int buyPrice;
    private int sellPrice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public static Optional<ItemType> GetHoldingType(GameObject gameObject)
    {
        if (Enum.TryParse(gameObject == null ? "" : gameObject.tag.Replace("Item", ""), out ItemType itemType)) return Optional<ItemType>.Of(itemType);
        else return Optional<ItemType>.Empty();
    }
}

public enum ItemType
{
    None, // Default value.
    Package,
    GlueCanister,
    Wood,
    Paper,
    EmptyBooks,
    Books,
    GlueBarrel,
}
