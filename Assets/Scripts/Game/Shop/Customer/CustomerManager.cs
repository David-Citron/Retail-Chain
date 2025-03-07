using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance = null;

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<Customer> customersActive;

    [SerializeField] private List<DisplaySlot> displayTables;

    [SerializeField] public Transform customerSpawnPoint;

    [SerializeField] private List<Transform> queuePointsTransforms;

    private List<CustomerPoint> customerPoints;
    private List<CustomerPoint> queuePoints;

    List<ActionTimer> timers;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        timers = new List<ActionTimer>();
        customerPoints = new List<CustomerPoint>();
        queuePoints = new List<CustomerPoint>();
        CreateCustomerPoints();
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

    private void CreateCustomerPoints()
    {
        customerPoints.Clear();
        queuePoints.Clear();
        // Find display table points
        for (int i = 0; i < displayTables.Count; i++)
        {
            if (displayTables[i] == null)
            {
                Debug.LogWarning("Display table is null!");
                continue;
            }
            List<GameObject> slots = displayTables[i].GetSlots();
            for (int j = 0; j < slots.Count; j++)
            {
                CustomerPoint newPoint = new CustomerPoint(displayTables[i].customerPoints[j].transform, displayTables[i], displayTables[i].GetSlots()[j]);
                customerPoints.Add(newPoint);
            }
        }
        // Find queue points
        for (int i = 0; i < queuePointsTransforms.Count; i++)
        {
            Debug.Log("ADDING CUSTOMER QUEUE POINT");
            CustomerPoint newPoint = new CustomerPoint(queuePointsTransforms[i]);
            queuePoints.Add(newPoint);
        }
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
        for (int i = 0; i < customerPoints.Count; i++)
        {
            Item foundItem = customerPoints[i].displayTable.GetItemFromSlot(customerPoints[i].itemSlot);
            if (foundItem == null)
                continue;
            ItemType itemType = foundItem.itemType;
            if (targetItemType == itemType)
            {
                targetTransform = customerPoints[i].point;
                displaySlot = customerPoints[i].displayTable;
                return true;
            }
        }
        return false;
    }

    public bool TryFindRandomDisplaySlot(out DisplaySlot displaySlot, out Transform targetTransform)
    {
        targetTransform = null;
        displaySlot = null;

        List<CustomerPoint> availablePoints = new List<CustomerPoint>();

        for (int i = 0; i < customerPoints.Count; i++)
        {
            if (customerPoints[i].reserved)
                continue;
            availablePoints.Add(customerPoints[i]);
        }

        if (availablePoints.Count == 0)
        {
            Debug.LogError("There are no available points!!!");
            return false;
        }

        int randomNumber = Random.Range(0, availablePoints.Count);
        targetTransform = availablePoints[randomNumber].point;
        displaySlot = availablePoints[randomNumber].displayTable;
        return true;
    }

    public CustomerPoint FindAvailableQueuePoint(Customer customer)
    {
        CustomerPoint customerPoint = null;
        bool found = false;
        for (int i = 0; i < queuePoints.Count; i++)
        {
            if (queuePoints[i].reserved)
                continue;
            queuePoints[i].reserved = true;
            customerPoint = queuePoints[i];
            found = true;
            break;
        }
        if (!found)
        {
            Debug.LogWarning("No available queue points found!");
            return null; // TODO
        }
        return customerPoint;
    }
}