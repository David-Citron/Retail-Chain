using System.Collections.Generic;
using UnityEngine;

public class PriceSystem : MonoBehaviour
{

    private Dictionary<ItemType, int> itemPrices;

    void Start()
    {
        itemPrices = new Dictionary<ItemType, int>();
    }

    void Update() {}

    
}
