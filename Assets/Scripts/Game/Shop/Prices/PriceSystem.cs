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
    /// Linearly interpolates the value based on the rating.
    /// </summary>
    /// <param name="basePrice">The base price</param>
    /// <returns>The max price</returns>
    public static int CalculateMaxPrice(int basePrice)
    {
        float rating = ShopRating.GetRating();
        int newPrice = TaxesManager.GetInflationPrice(basePrice);

        float[] ratingLevels = { 0, 1, 2, 3, 4, 5 };
        float[] modifiers = { -0.15f, -0.10f, -0.05f, 0.0f, 0.02f, 0.05f }; //Price modifiers based on the rating level

        int lowerIndex = Mathf.FloorToInt(rating);
        int upperIndex = Mathf.CeilToInt(rating);

        lowerIndex = Mathf.Clamp(lowerIndex, 0, ratingLevels.Length - 1); //That assures that the index is <= 5 & >= 0
        upperIndex = Mathf.Clamp(upperIndex, 0, ratingLevels.Length - 1);

        float lowerModifier = modifiers[lowerIndex];
        float upperModifier = modifiers[upperIndex];

        float modifier = Mathf.Lerp(lowerModifier, upperModifier, rating - lowerIndex); //Linearly interpolate the value

        newPrice = (int) (newPrice * (1 + modifier));
        return newPrice;
    }

    /// <summary>
    /// Recommended price will be shown to player and is lowered by 5%.
    /// </summary>
    /// <param name="basePrice">The base price</param>
    /// <returns>The max price reduced by 5%</returns>
    public static int CalculateRecommendedPrice(int basePrice) => (int)(CalculateMaxPrice(basePrice) * 0.95d);
    public static int CalculateRecommendedPrice(ItemType itemType) => CalculateRecommendedPrice(ItemManager.GetItemData(itemType).sellPrice);
}
