using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContractItemData : MonoBehaviour
{
    [SerializeField] private ItemType type;
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private TMP_InputField priceInput;
    [SerializeField] private TMP_Text recommendedPrice;
    [SerializeField] private RawImage iconImage;
    [SerializeField] private TMP_Text itemText;
    [SerializeField] private List<GameObject> extraButtons;
    [SerializeField] private TMP_Text sumText;

    private byte amount;
    private int price;
    private int sum;

    // Start is called before the first frame update
    void Start()
    {
        amount = 0;
        price = 0;
        sum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Fills in the data of passed contract item object
    public void LoadData(ContractItem contractItem)
    {
        type = contractItem.itemType;
        if (contractItem.quantity < 0) contractItem.quantity = 0;
        else if (contractItem.quantity > 255) contractItem.quantity = 255;
        amount = (byte)contractItem.quantity;
        price = contractItem.price;
        CalculateSum();
        UpdatePriceField();
        UpdateAmountField();
        UpdateSum();
        amountInput.interactable = false;
        priceInput.interactable = false;
        itemText.text = ItemManager.GetNameOf(contractItem.itemType);
        iconImage.texture = ItemManager.GetIcon(contractItem.itemType);
        extraButtons.ForEach(button => button.SetActive(false));
        recommendedPrice.text = PriceSystem.CalculateRecommendedPrice(price) + "$";
    }

    // This method is for initializing list without values - only icon and name
    public void LoadData(ItemType itemType)
    {
        type = itemType;
        amount = 0;
        price = 0;
        sum = 0;
        UpdatePriceField();
        UpdateAmountField();
        UpdateSum();
        amountInput.interactable = true;
        priceInput.interactable = true;
        itemText.text = ItemManager.GetNameOf(itemType);
        iconImage.texture = ItemManager.GetIcon(itemType);
        extraButtons.ForEach(button => button.SetActive(true));
        recommendedPrice.text = PriceSystem.CalculateRecommendedPrice(price) + "$";
    }

    // Retrieve data from UI
    public ContractItem ReadData()
    {
        string amount = amountInput.text;
        string price = priceInput.text;
        ContractItem contractItem = new ContractItem(type, int.Parse(amount), int.Parse(price));
        return contractItem;
    }

    public void IncreaseAmount()
    {
        if (amount == 255) return;
        amount++;
        UpdateAmountField();
        CalculateSum();
        UpdateSum();
    }

    public void DecreaseAmount()
    {
        if (amount == 0) return;
        amount--;
        UpdateAmountField();
        CalculateSum();
        UpdateSum();
    }

    public void ReadAmount(string input)
    {
        byte newAmount = 0;
        byte.TryParse(input, out newAmount);
        amount = newAmount;
        UpdateAmountField();
        CalculateSum();
        UpdateSum();
    }

    public void ReadPrice(string input)
    {
        int newPrice = 0;
        int.TryParse(input, out newPrice);
        price = newPrice;
        UpdatePriceField();
        CalculateSum();
        UpdateSum();
    }

    public void UpdateAmountField() => amountInput.text = "" + amount;
    public void UpdatePriceField() => priceInput.text = "" + price;

    public void CalculateSum() => sum = price * (int)amount;
    public void UpdateSum() => sumText.text = "" + sum;
}
