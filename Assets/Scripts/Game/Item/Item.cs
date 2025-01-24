using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemType itemType = ItemType.None;
    public ItemType contentType = ItemType.None;
    public int sellPrice = 0;

    void Start() {}
    void Update() {}

}

[System.Serializable]
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
