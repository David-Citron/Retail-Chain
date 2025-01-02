using System;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int buyPrice;
    private int sellPrice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public static GameObject GetGameObjectFromPrefab(ItemType type)
    {
        if (type == ItemType.None) return null;

        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Items/" + type + ".prefab", typeof(GameObject));
        if (prefab == null) return null;
        return (GameObject) Instantiate(prefab);
    }
}

public enum ItemType
{
    None, // Default value.
    Package,
    GlueCanister,
    Wood,
    Paper,
    EmptyBook,
    Book,
    GlueBarrel,
}
