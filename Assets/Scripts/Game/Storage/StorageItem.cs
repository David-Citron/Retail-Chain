using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StorageContentItem : MonoBehaviour
{

    public ItemType itemType;
    public int amount;

    public RawImage icon;
    public TMP_Text itemName;
    public Button takeButton;

    void Start() {}
    void Update() {}

    public void Initialize(ItemType itemType, int amount, bool tester)
    {
        this.itemType = itemType;
        this.amount = amount;

        itemName.text = ItemManager.GetNameOf(itemType) + "\n" + amount + "x";
        icon.texture = ItemManager.GetIcon(itemType);

        takeButton.enabled = true;
        takeButton.interactable = true;
        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(() =>
        {
            ItemStorage.instance.TakeItem(itemType, tester);
        });
    }

    public void Initialize(ItemType itemType, int amount) => Initialize(itemType, amount, true);
}
