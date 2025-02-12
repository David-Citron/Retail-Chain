using UnityEngine;
using System.Collections.Generic;

public class TestingUtils : MonoBehaviour
{
    public static TestingUtils instance;

    private List<GameObject> items = new List<GameObject>();

    public GameObject itemListContent;
    public GameObject testerUI;
    public GameObject prefabItem;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.T)) return;
        testerUI.SetActive(!testerUI.activeSelf);

        if (!testerUI.activeSelf) return;

        foreach (var item in items) Destroy(item);
        foreach (var item in ItemManager.GetAllItemData())
        {
            GameObject createdItem = Instantiate(prefabItem);

            createdItem.transform.SetParent(itemListContent.transform);
            createdItem.transform.localScale = Vector3.one;

            var component = createdItem.GetComponent<StorageContentItem>();

            component.Initialize(item.itemType, 1, false);

            items.Add(createdItem);
        }
    }
}
