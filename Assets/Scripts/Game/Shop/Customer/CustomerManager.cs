using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance = null;

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<GameObject> browsingPoints;
    [SerializeField] private List<GameObject> payQueue;
    [SerializeField] private List<Customer> customersActive;
    [SerializeField] public List<GameObject> displayTables;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        GameObject customerObject = Instantiate(customerPrefab);
        Customer customer = customerObject.GetComponent<Customer>();
        if (customer == null)
        {
            Debug.LogError("Customer script not found");
            return;
        }
        customersActive.Add(customer);
    }
}
