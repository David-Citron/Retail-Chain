using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static List<ItemData> items = new List<ItemData>();

    void Start() {}
    void Update() {}


    public static GameObject CreateItem(ItemType itemType)
    {
        GameObject gameObject = GetGameObjectFromPrefab(itemType);
        Item item = gameObject.AddComponent<Item>();
        item.itemType = itemType;

        return gameObject;
    }


    public static void UpdateItem(GameObject gameObject, ItemType contentType) => UpdateItem(gameObject, contentType, 0);
    public static void UpdateItem(GameObject gameObject, int sellPrice) => UpdateItem(gameObject, ItemType.None, sellPrice);

    public static void UpdateItem(GameObject gameObject, ItemType contentType, int sellPrice)
    {
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
        return Instantiate(items.Find(item => item.itemType == type).itemPrefab);
    }

    public static string GetNameOf(ItemType type)
    {
        if (type == ItemType.None) return "Error";
        return items.Find(item => item.itemType == type).itemName;
    }


    public static RenderTexture GetIcon(ItemType type)
    {
        if (type == ItemType.None) return null;
        return items.Find(item => item.itemType == type).icon;
    }
}
