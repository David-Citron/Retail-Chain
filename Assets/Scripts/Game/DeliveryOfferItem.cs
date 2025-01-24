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

    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Button increment;
    [SerializeField] private Button decrease;

    public void Initialize(DeliveryOffer deliveryOffer)
    {
        this.deliveryOffer = deliveryOffer;
    }

    void Start() {
        ItemData itemData = ItemManager.GetItemData(deliveryOffer.item);
        itemIcon.texture = itemData.icon;
        itemName.text = itemData.name;
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
    }

    void Update() {}

    private void UpdateAmount(int by)
    {
        amountToBuy = Mathf.Clamp(amountToBuy + by, 0, deliveryOffer.itemAmount);
        amountText.text = amountToBuy + "/" + deliveryOffer.itemAmount;
    }
}
