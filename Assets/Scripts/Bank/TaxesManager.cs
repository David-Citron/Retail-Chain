using Mirror;

public class TaxesManager : NetworkBehaviour
{
    private static TaxesManager instance;

    private int taxesTariff;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        
    }

    public void IncreaseTariff(int increaseBy)
    {
        taxesTariff += increaseBy;
    }


    public static int GetTaxedPrice(int price)
    {
        int newPrice = price;
        Instance().IfPresent(taxesManager => newPrice = price + (int) (price * (taxesManager.taxesTariff / 100m)));
        return newPrice;
    }

    public static Optional<TaxesManager> Instance() => Optional<TaxesManager>.Of(instance);
}
