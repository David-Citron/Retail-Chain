using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance = null;

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<Customer> customersActive;

    [SerializeField] private List<DisplayTable> displayTables;

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

    public CustomerPoint TryFindDisplaySlot(ItemType targetItemType)
    {
        for (int i = 0; i < customerPoints.Count; i++)
        {
            if (customerPoints[i].reserved)
                continue;
            Item foundItem = customerPoints[i].displayTable.GetItemFromSlot(customerPoints[i].itemSlot);
            if (foundItem == null)
                continue;
            ItemType itemType = foundItem.itemType;
            if (targetItemType != itemType)
                continue;
            return customerPoints[i];
        }
        return null;
    }

    public CustomerPoint TryFindRandomDisplaySlot()
    {
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
            return null;
        }

        int randomNumber = Random.Range(0, availablePoints.Count);
        return customerPoints[customerPoints.IndexOf(availablePoints[randomNumber])];
    }

    public CustomerPoint FindAvailableQueuePoint(Customer customer)
    {
        CustomerPoint customerPoint = null;
        bool found = false;
        for (int i = 0; i < queuePoints.Count; i++)
        {
            if (queuePoints[i].reserved)
                continue;
            customerPoint = queuePoints[i];
            found = true;
            break;
        }
        if (!found)
        {
            Debug.LogWarning("No available queue points found!");
            return null;
        }
        return customerPoint;
    }

    public bool MakeReservation(CustomerPoint target, Customer customer)
    {
        int resultIndex = customerPoints.IndexOf(target);
        if (resultIndex == -1)
        {
            resultIndex = queuePoints.IndexOf(target);
            if (resultIndex == -1)
            {
                Debug.LogError("Target was NOT found!");
                return false;
            }
            if (queuePoints[resultIndex] == null)
            {
                Debug.LogError("Target CustomerPoint was not found!");
                return false;
            }
            if (queuePoints[resultIndex].reserved)
            {
                Debug.LogError("Point is already reserved!");
                return false;
            }
            queuePoints[resultIndex].reserved = true;
            queuePoints[resultIndex].reservedCustomer = customer;
            return true;
        }

        if (customerPoints[resultIndex] == null)
        {
            Debug.LogError("Target CustomerPoint was not found!");
            return false;
        }
        if (customerPoints[resultIndex].reserved)
        {
            Debug.LogError("Point is already reserved!");
            return false;
        }
        customerPoints[resultIndex].reserved = true;
        customerPoints[resultIndex].reservedCustomer = customer;
        return true;
    }

    public bool CancelReservation(CustomerPoint target)
    {
        int resultIndex = customerPoints.IndexOf(target);
        if (resultIndex == -1)
        {
            resultIndex = queuePoints.IndexOf(target);
            if (resultIndex == -1)
            {
                Debug.LogError("Target was NOT found!");
                return false;
            }
            if (queuePoints[resultIndex] == null)
            {
                Debug.LogError("Target CustomerPoint was not found!");
                return false;
            }
            if (!queuePoints[resultIndex].reserved)
            {
                Debug.LogError("Point is NOT reserved by anyone!");
                return false;
            }
            queuePoints[resultIndex].reserved = false;
            queuePoints[resultIndex].reservedCustomer = null;
            return true;
        }

        if (customerPoints[resultIndex] == null)
        {
            Debug.LogError("Target CustomerPoint was not found!");
            return false;
        }
        if (!customerPoints[resultIndex].reserved)
        {
            Debug.LogError("Point is NOT reserved by anyone!");
            return false;
        }
        customerPoints[resultIndex].reserved = false;
        customerPoints[resultIndex].reservedCustomer = null;
        return true;
    }

    public void UpdateQueue()
    {
        // FIX this later - must move the line, not override
        customersActive.ForEach(customer => customer.QueueUpdate());
    }

    public Item GetItemFromSlot(CustomerPoint point)
    {
        int i = customerPoints.IndexOf(point);
        if (customerPoints[i] == null)
            return null;
        GameObject itemSlot = customerPoints[i].itemSlot;
        return customerPoints[i].displayTable.GetItemFromSlot(customerPoints[i].itemSlot);
    }

    public bool RemoveItemFromSlot(CustomerPoint point)
    {
        int i = customerPoints.IndexOf(point);
        if (customerPoints[i] == null)
            return false;
        GameObject itemSlot = customerPoints[i].itemSlot;
        return customerPoints[i].displayTable.RemoveItemFromSlot(customerPoints[i].itemSlot);
    }
}