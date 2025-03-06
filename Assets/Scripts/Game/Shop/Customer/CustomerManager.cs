using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance = null;

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<GameObject> browsingPoints;
    [SerializeField] private List<GameObject> payQueue;
    [SerializeField] private List<Customer> customersActive;

    [SerializeField] public List<Transform> displayTablePoints; // shouldn't be public TODO
    [SerializeField] private List<DisplaySlot> displayTables;

    [SerializeField] private Transform customerSpawnPoint;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnNewCustomer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnNewCustomer()
    {
        if (customerPrefab == null)
        {
            Debug.LogError("Customer prefab is not set");
            return;
        }
        GameObject customerObject = Instantiate(customerPrefab, customerSpawnPoint);
        customerObject.transform.localPosition = Vector3.zero;
        Customer customer = customerObject.GetComponent<Customer>();
        if (customer == null)
        {
            Debug.LogError("Customer script not found");
            return;
        }
        customersActive.Add(customer);
    }

    public Transform GetDisplayTableWithItem(ItemType targetItemType)
    {
        Transform target = null;
        for (int i = 0; i < displayTables.Count - 1; i++)
        {
            List<GameObject> slots = displayTables[i].GetCurrentItems();
            for (int j = 0; j < slots.Count - 1; j++)
            {
                ItemType itemType = ItemManager.GetItemType(slots[i]).GetValueOrDefault();
                if (targetItemType == itemType)
                {
                    target = displayTables[i].customerPoints[j];
                    break;
                }
            }
        }
        return target;
    }

    public Transform GetRandomDisplayTable()
    {
        Transform target = null;
        int randomNumberTable = Random.Range(0, displayTables.Count);
        int randomNumberSlot = Random.Range(0, displayTables[randomNumberTable].GetCurrentItems().Count);
        target = displayTables[randomNumberTable].customerPoints[randomNumberSlot];
        return target;
    }
}
