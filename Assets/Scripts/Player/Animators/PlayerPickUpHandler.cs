using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpHandler: MonoBehaviour
{
    private Animator animator;

    [SerializeField] private List<GameObject> itemsInRange = new List<GameObject>();
    [SerializeField] private List<GameObject> itemsInInventory = new List<GameObject>();

    private void Start()
    {
        animator = GetComponent<Animator>();
        itemsInRange.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemsInRange.Count == 0) return;
            GameObject nearestItem = itemsInRange[0];
            float nearestItemDistance = Vector3.Distance(transform.position, nearestItem.transform.position);
            for (int i = 0; i < itemsInRange.Count; i++)
            {
                float currentItemDistance = Vector3.Distance(transform.position, itemsInRange[i].transform.position);
                if (currentItemDistance > nearestItemDistance) break;
                nearestItemDistance = currentItemDistance;
                nearestItem = itemsInRange[i];
            }
            PickUp(nearestItem);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (MachineManager.IsGameObjectInMachine(other.gameObject)) return;
        itemsInRange.Add(other.gameObject);
        // Debug.Log("New item reachable: " + other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        itemsInRange.Remove(other.gameObject);
    }

    private void PickUp(ItemType item)
    {
        Debug.Log(item);
        animator.SetBool("holding", true);

        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            if (i == (int) item) itemsInInventory[i].SetActive(true);
            else itemsInInventory[i].SetActive(false);
        }
    }

    private void PickUp(GameObject itemGameObject)
    {
        PickUp((ItemType) Enum.Parse(typeof(ItemType), itemGameObject.tag.Replace("Item", "")));
        Destroy(itemGameObject);
    }
}
