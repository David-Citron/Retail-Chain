using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpHandler: MonoBehaviour
{
    private Animator animator;

    private List<GameObject> itemsInRange = new List<GameObject>();
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
        itemsInRange.Add(other.gameObject);
        // Debug.Log("New item reachable: " + other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        itemsInRange.Remove(other.gameObject);
    }

    private void PickUp (ItemType item)
    {
        animator.SetBool("holding", true);
        switch (item)
        {
            case ItemType.Package:
                for (int i = 0; i < itemsInInventory.Count; i++)
                {
                    if (i == 0)itemsInInventory[i].SetActive(true);
                    else itemsInInventory[i].SetActive(false);
                }
                break;
        }
    }

    private void PickUp (GameObject itemGameObject)
    {
        switch(itemGameObject.tag)
        {
            case "ItemPackage":
                PickUp(ItemType.Package);
                break;
        }
        Destroy(itemGameObject);
    }
}
