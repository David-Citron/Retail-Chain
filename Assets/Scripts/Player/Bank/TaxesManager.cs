using Mirror;
using UnityEngine;

public class TaxesManager : NetworkBehaviour
{
    public static TaxesManager instance;

    [SyncVar]
    public float inflation;

    void Start()
    {
        instance = this;
        syncDirection = SyncDirection.ServerToClient;
        inflation = 1;
    }

    void Update() {}

    /// <summary>
    /// Increases inflation by given number
    /// </summary>
    /// <param name="increaseBy">Inflation will increase by that amount</param>
    public static void IncraseInflation(float increaseBy) => instance.inflation += increaseBy;

    /// <summary>
    /// Returns price multiplied by current inflation.
    /// </summary>
    /// <param name="price">The price</param>
    /// <returns>The new price (inflation included)</returns>
    public static int GetInflationPrice(int price) => (int) (price * GetInflation());


    /// <summary>
    /// Returns price multiplied by current inflation that is decreased by some number.
    /// This is primarily for goods tha
    /// </summary>
    /// <param name="price">The price</param>
    /// <returns>The new price (inflation included)</returns>
    public static int GetGoodsInflationPrice(int price)
    {
        float randomMultiplier = 1 + Random.Range(-0.15f, 0.1f); // Random value between 0.85 and 1.15
        return (int)(price * Mathf.Clamp((GetInflation() * 0.75f), 1, GetInflation()) * randomMultiplier);
    }


    public static float GetInflation() => instance == null ? 1 : instance.inflation;
}
