using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    void Start() { instance = this; }
    void Update() {}


    public static GameObject CreateItem(ItemType itemType)
    {
        GameObject gameObject = GetGameObjectFromPrefab(itemType);
        if (gameObject == null) return null;
        Item item = gameObject.AddComponent<Item>();
        item.itemType = itemType;

        return gameObject;
    }

    public static List<ItemData> GetAllItemData() => instance.items;

    public static Item GetItemInfo(GameObject gameObject) => gameObject == null ? null : gameObject.GetComponent<Item>();

    public static void UpdateItem(GameObject gameObject, ItemType contentType) => UpdateItem(gameObject, contentType, 0);
    public static void UpdateItem(GameObject gameObject, int sellPrice) => UpdateItem(gameObject, ItemType.None, sellPrice);

    public static void UpdateItem(GameObject gameObject, ItemType contentType, int sellPrice)
    {
        if(gameObject == null) return;
        Item item = gameObject.GetComponent<Item>();
        if (item == null) return;
        item.contentType = contentType;
        item.sellPrice = sellPrice;
    }

    public static Optional<ItemType> GetItemType(GameObject gameObject)
    {
        if (Enum.TryParse(gameObject == null ? "" : gameObject.tag.Replace("Item", ""), out ItemType itemType)) return Optional<ItemType>.Of(itemType);
        else return Optional<ItemType>.Empty();
    }

    /// <summary>
    /// Gets the prefab item based on the ItemType.
    /// </summary>
    /// <param name="type">The ItemType</param>
    /// <returns>New gameobject</returns>
    private static GameObject GetGameObjectFromPrefab(ItemType type)
    {
        if (type == ItemType.None) return null;
        return Instantiate(GetItemData(type).itemPrefab);
    }

    public static string GetNameOf(ItemType type)
    {
        if (type == ItemType.None) return "Error";
        return GetItemData(type).itemName;
    }


    public static RenderTexture GetIcon(ItemType type)
    {
        if (type == ItemType.None) return null;
        return GetItemData(type).icon;
    }

    public static ItemData GetItemData(ItemType itemType) => instance.items.Find(item => item.itemType == itemType);
}
