using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpHandler: MonoBehaviour
{

    [SerializeField] private GameObject playerHands;
    private GameObject holdingItem;

    private Animator animator;

    private List<GameObject> itemsInRange = new List<GameObject>();

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
        } else if(Input.GetKeyDown(KeyCode.Q))
        {
            DropHoldingItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (MachineManager.IsGameObjectInMachine(other.gameObject)) return;
        itemsInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        itemsInRange.Remove(other.gameObject);
    }

    private void PickUp(GameObject itemGameObject)
    {
        if (holdingItem != null) return;

        animator.SetBool("holding", true);
        itemGameObject.transform.SetParent(playerHands.transform);
        itemGameObject.transform.localPosition = Vector3.zero;
        holdingItem = itemGameObject;
    }

    private void DropHoldingItem()
    {
        if (holdingItem == null) return;

        animator.SetBool("holding", false);
        holdingItem.transform.SetParent(playerHands.transform);
        holdingItem = null;
    }
}
