using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PriceItem : MonoBehaviour
{
    [SerializeField] private RawImage itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text recommendedPrice;
    [SerializeField] private TMP_InputField priceInput;

    void Start() {}
    void Update() {}

    public void Initialize(ItemData itemData)
    {
        itemIcon.texture = itemData.icon;
        itemName.text = itemData.itemName;
        priceInput.text = "$" + PriceSystem.GetPrice(itemData.itemType);

        recommendedPrice.text = "$" + PriceSystem.CalculateRecommendedPrice(itemData.sellPrice);

        priceInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        priceInput.onEndEdit.AddListener(newValue =>
        {
            if (!int.TryParse(newValue, out int newPrice)) return;
            PriceSystem.UpdatePrice(itemData.itemType, newPrice);
            priceInput.text = "$" + newPrice;
        });
    }
}
