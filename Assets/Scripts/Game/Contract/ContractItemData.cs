using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContractItemData : MonoBehaviour
{
    [SerializeField] private ItemType type;
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private TMP_InputField priceInput;
    [SerializeField] private RawImage iconImage;
    [SerializeField] private TMP_Text itemText;
    [SerializeField] private List<GameObject> extraButtons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Fills in the data of passed contract item object
    public void LoadData(ContractItem contractItem)
    {
        amountInput.text = "" + contractItem.quantity;
        amountInput.interactable = false;
        priceInput.text = "" + contractItem.price;
        priceInput.interactable = false;
        itemText.text = ItemManager.GetNameOf(contractItem.itemType);
        iconImage.texture = ItemManager.GetIcon(contractItem.itemType);
        type = contractItem.itemType;
        extraButtons.ForEach(button => button.SetActive(false));
    }

    // This method is for initializing list without values - only icon and name
    public void LoadData(ItemType itemType)
    {
        amountInput.text = "0";
        amountInput.interactable = true;
        priceInput.text = "0";
        priceInput.interactable = true;
        itemText.text = ItemManager.GetNameOf(itemType);
        iconImage.texture = ItemManager.GetIcon(itemType);
        type = itemType;
        extraButtons.ForEach(button => button.SetActive(true));
    }

    // Retrieve data from UI
    public ContractItem ReadData()
    {
        string amount = amountInput.text;
        string price = priceInput.text;
        ContractItem contractItem = new ContractItem(type, int.Parse(amount), int.Parse(price));
        return contractItem;
    }
}
