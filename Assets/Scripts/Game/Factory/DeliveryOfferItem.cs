using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryOfferItem : MonoBehaviour
{
    public DeliveryOffer deliveryOffer;
    private int amountToBuy;

    [SerializeField] private RawImage itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemPrice;

    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private Button increment;
    [SerializeField] private Button decrease;

    [SerializeField] private Button buyButton;


    public void Initialize(DeliveryOffer deliveryOffer)
    {
        this.deliveryOffer = deliveryOffer;
        UpdateAmount();
    }

    void Start() {
        amountInput.onValueChanged.AddListener(newValue => CheckValue(newValue));
        amountInput.characterValidation = TMP_InputField.CharacterValidation.Integer;

        itemIcon.texture = deliveryOffer.item.icon;
        itemName.text = deliveryOffer.item.name;
        itemPrice.text = "$" + deliveryOffer.price + " / item";

        increment.gameObject.SetActive(true);
        increment.onClick.RemoveAllListeners();
        increment.onClick.AddListener(() =>
        {
            UpdateAmount(1);
        });

        decrease.gameObject.SetActive(true);
        decrease.onClick.RemoveAllListeners();
        decrease.onClick.AddListener(() =>
        {
            UpdateAmount(-1);
        });

        buyButton.gameObject.SetActive(true);
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyItems());
    }

    void Update() {}

    private void UpdateAmount(int by)
    {
        amountToBuy = Mathf.Clamp(amountToBuy + by, 0, deliveryOffer.itemAmount);
        amountInput.placeholder.name = amountToBuy + "/" + deliveryOffer.itemAmount;
        amountInput.text = amountToBuy + "/" + deliveryOffer.itemAmount;
    }

    private void SetAmount(int amount)
    {
        amountToBuy = 0;
        amountInput.placeholder.name = amountToBuy + "/" + deliveryOffer.itemAmount;
        amountInput.text = amountToBuy + "/" + deliveryOffer.itemAmount;
    }

    private void UpdateAmount() => UpdateAmount(0);

    private void BuyItems()
    {
        deliveryOffer.itemAmount = deliveryOffer.itemAmount - amountToBuy;
        StorageRack.instance.InsertItem(deliveryOffer.item.itemType, amountToBuy);

        SetAmount(0); //To set 0 to buy

        if (deliveryOffer.itemAmount > 0) return;
        GoodsDelivery.instance.UpdateOffers();
    }


    private void CheckValue(string newValue)
    {
        string[] parts = newValue.Split('/');
        string result = parts[0];
        if (!int.TryParse(result, out int newAmount))
        {

            return;
        }

        amountToBuy = newAmount;
        UpdateAmount();
    }
}
