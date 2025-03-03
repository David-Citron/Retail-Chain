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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(ContractItem item)
    {
        if (item.quantity == 0) Destroy(gameObject);

        itemLabel.text = ItemManager.GetNameOf(item.itemType);
        itemIcon.texture = ItemManager.GetIcon(item.itemType);
        amount.text = (item.quantity - item.quantityRemaining) + "/" + item.quantity;
        if (item.fulfilled)
            statusIcon.texture = textureSuccess;
        else 
            statusIcon.texture = textureFailure;
    }
}
