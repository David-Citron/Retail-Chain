using Mirror;

public class TaxesManager : NetworkBehaviour
{
    public static TaxesManager instance;

    public const int DEFAULT_RENT_TAXES = 50;

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
    /// Calculates rent taxes. That includes electricity, gas & rent.
    /// There is default rate which is multiplied by inflation.
    /// </summary>
    /// <returns>The rent taxes</returns>
    public static int GetRentTaxes() => GetInflationPrice(DEFAULT_RENT_TAXES);

    /// <summary>
    /// Returns price multiplied by current inflation.
    /// </summary>
    /// <param name="price">The price</param>
    /// <returns>The new price (inflation included)</returns>
    public static int GetInflationPrice(int price) => (int) (price * instance.inflation);
}
