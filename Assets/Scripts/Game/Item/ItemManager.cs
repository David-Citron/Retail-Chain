using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public List<ItemData> items = new List<ItemData>();

    void Start() { instance = this; }
    void Update() {}


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
    public static GameObject GetGameObjectFromPrefab(ItemType type)
    {
        if (type == ItemType.None) return null;
        return Instantiate(instance.items.Find(item => item.itemType == type).itemPrefab);
    }

    public static string GetNameOf(ItemType type)
    {
        if (type == ItemType.None) return "Error";
        return instance.items.Find(item => item.itemType == type).itemName;
    }


    public static RenderTexture GetIcon(ItemType type)
    {
        if (type == ItemType.None) return null;
        return instance.items.Find(item => item.itemType == type).icon;
    }
}
