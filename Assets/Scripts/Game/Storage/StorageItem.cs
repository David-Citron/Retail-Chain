using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageContentItem : MonoBehaviour
{

    public ItemType itemType;
    public int amount;

    public RawImage icon;
    public TMP_Text itemName;
    public Button takeButton;

    void Start() {}
    void Update() {}

    public void Initialize(ItemType itemType, int amount)
    {
        this.itemType = itemType;
        this.amount = amount;

        itemName.text = Item.GetNameOf(itemType);

        takeButton.enabled = true;
        takeButton.interactable = true;
        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(() =>
        {
            ItemStorage.instance.TakeItem(itemType);
        });
    }
}
