using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemType itemType { get; set; }
    public ItemType contentType = ItemType.None;
    public int sellPrice { get; set; }

    void Start() {}
    void Update() {}

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
