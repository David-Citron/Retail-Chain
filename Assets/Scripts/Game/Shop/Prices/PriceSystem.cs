using System.Collections.Generic;
using UnityEngine;

public class PriceSystem : MonoBehaviour
{
    private static Dictionary<ItemType, int> itemPrices;

    void Start()
    {
        itemPrices = new Dictionary<ItemType, int>();

        ItemManager.GetAllSellableItemData().ForEach(item => itemPrices.Add(item.itemType, CalculateRecommendedPrice(item.sellPrice)));
    }

    void Update() {}

    public static int GetPrice(ItemType itemType) => itemPrices[itemType];
    public static void UpdatePrice(ItemType itemType, int newPrice)
    {
        itemPrices[itemType] = newPrice;

        if (CashRegister.instance == null) return;
        CashRegister.instance.UpdateContent(true);
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
                modifier = 0;
                break;
            case 4:
                modifier = 0.02f;
                break;
            case 5:
                modifier = 0.05f;
                break;
            default:
                modifier = 0;
                break;
        }

        newPrice = (int)(newPrice * (1 + modifier));
        return newPrice;
    }

    /// <summary>
    /// Recommended price will be shown to player and is lowered by 5%.
    /// </summary>
    /// <param name="basePrice">The base price</param>
    /// <returns>The max price reduced by 5%</returns>
    public static int CalculateRecommendedPrice(int basePrice) => (int)(CalculateMaxPrice(basePrice) * 0.95d);
}
