using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPoint
{
    public Transform point;
    public DisplaySlot displayTable;
    public GameObject itemSlot;
    public bool reserved;
    public Customer reservedCustomer;

    /// <summary>
    /// Creates a new CustomerPoint which is not reserved
    /// </summary>
    public CustomerPoint(Transform point, DisplaySlot displayTable, GameObject itemSlot)
    {
        this.point = point;
        this.displayTable = displayTable;
        this.itemSlot = itemSlot;
        reserved = false;
        reservedCustomer = null;
    }

    /// <summary>
    /// Creates a new CustomerPoint and allows specifying reservation
    /// </summary>
    public CustomerPoint(Transform point, DisplaySlot displayTable, GameObject itemSlot, bool reserved, Customer reservedCustomer)
    {
        this.point = point;
        this.displayTable = displayTable;
        this.itemSlot = itemSlot;
        this.reserved = reserved;
        this.reservedCustomer = reservedCustomer;
    }

    /// <summary>
    /// Creates a new CustomerPoint for queue
    /// </summary>
    public CustomerPoint(Transform point)
    {
        this.point = point;
        displayTable = null;
        itemSlot = null;
        reserved = false;
        reservedCustomer = null;
    }
}
