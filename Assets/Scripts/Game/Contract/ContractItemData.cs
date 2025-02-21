using System;
using System.Collections;
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
        priceInput.text = "" + contractItem.price;
        itemText.text = ItemManager.GetNameOf(contractItem.itemType);
        iconImage.texture = ItemManager.GetIcon(contractItem.itemType);
        type = contractItem.itemType;
    }

    // This method is for initializing list without values - only icon and name
    public void LoadData(ItemType itemType)
    {
        amountInput.text = "0";
        priceInput.text = "0";
        itemText.text = ItemManager.GetNameOf(itemType);
        iconImage.texture = ItemManager.GetIcon(itemType);
        type = itemType;
    }

    // Retrieve data from UI
    public ContractItem ReadData()
    {
        string amount = amountInput.text;
        string price = priceInput.text;
        ContractItem contractItem = new ContractItem(type, Int32.Parse(amount), Int32.Parse(price));
        return contractItem;
    }
}
