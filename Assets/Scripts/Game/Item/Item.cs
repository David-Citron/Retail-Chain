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
