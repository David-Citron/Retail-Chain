using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemainingContractData : MonoBehaviour
{
    [SerializeField] private TMP_Text itemLabel;
    [SerializeField] private RawImage itemIcon;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private RawImage statusIcon;
    [SerializeField] private Texture textureSuccess;
    [SerializeField] private Texture textureFailure;
    [SerializeField] private TMP_Text priceLabel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(ContractItem item, PlayerRole role)
    {
        if (item.quantity == 0) Destroy(gameObject);
        switch (role)
        {
            case PlayerRole.Factory:
                priceLabel.gameObject.SetActive(false);
                itemLabel.gameObject.SetActive(true);
                itemIcon.gameObject.SetActive(true);
                amount.gameObject.SetActive(true);
                statusIcon.gameObject.SetActive(true);
                itemLabel.text = ItemManager.GetNameOf(item.itemType);
                itemIcon.texture = ItemManager.GetIcon(item.itemType);
                amount.text = (item.quantity - item.quantityRemaining) + "/" + item.quantity;
                if (item.fulfilled)
                    statusIcon.texture = textureSuccess;
                else
                    statusIcon.texture = textureFailure;
                break;
            case PlayerRole.Shop:
                priceLabel.gameObject.SetActive(true);
                itemLabel.gameObject.SetActive(true);
                itemIcon.gameObject.SetActive(true);
                amount.gameObject.SetActive(false);
                statusIcon.gameObject.SetActive(false);
                itemLabel.text = item.quantity + "x " + ItemManager.GetNameOf(item.itemType);
                itemIcon.texture = ItemManager.GetIcon(item.itemType);
                priceLabel.text = item.price + "$";
                break;
            default:
                Debug.LogError("Loading data for unassigned player role!");
                break;
        }
    }
}
