using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance = null;

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<GameObject> browsingPoints;
    [SerializeField] private List<GameObject> payQueue;
    [SerializeField] private List<Customer> customersActive;

    [SerializeField] private List<DisplaySlot> displayTables;

    [SerializeField] public Transform customerSpawnPoint;

    List<ActionTimer> timers;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        timers = new List<ActionTimer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnNewCustomer();
        CreateNewCustomerTimer();
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

    public void CustomerLeaving()
    {
        Debug.Log("Callback for leaving called!");
        CreateNewCustomerTimer();
    }

    private void CreateNewCustomerTimer()
    {
        float time = Random.Range(3.0f, 7.0f);
        ActionTimer timer = null;
        timer = new ActionTimer(() =>
        {
            SpawnNewCustomer();
            timer.Stop();
            timers.Remove(timer);
        }, 5).Run();
        timers.Add(timer);
    }

    public bool TryFindDisplaySlot(ItemType targetItemType, out DisplaySlot displaySlot, out Transform targetTransform)
    {
        targetTransform = null;
        displaySlot = null;
        for (int i = 0; i < displayTables.Count; i++)
        {
            List<GameObject> slots = displayTables[i].GetCurrentItems();
            for (int j = 0; j < slots.Count; j++)
            {
                ItemType itemType = ItemManager.GetItemType(slots[j]).GetValueOrDefault();
                if (targetItemType == itemType)
                {
                    targetTransform = displayTables[i].GetItemFromSlot(slots[j]).transform;
                    displaySlot = displayTables[i];
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryFindRandomDisplaySlot(out DisplaySlot displaySlot, out Transform targetTransform)
    {
        targetTransform = null;
        displaySlot = null;
        int randomNumberTable = Random.Range(0, displayTables.Count);
        int randomNumberSlot = Random.Range(0, displayTables[randomNumberTable].customerPoints.Count);
        targetTransform = displayTables[randomNumberTable].customerPoints[randomNumberSlot];
        displaySlot = displayTables[randomNumberTable];
        return targetTransform != null && displaySlot != null;
    }
}
