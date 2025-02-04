using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public RenderTexture icon;
    public GameObject itemPrefab;
    public int maxOfferAmount;

    public int buyPrice;
    public int sellPrice;

    public bool sellable;
}