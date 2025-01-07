using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private static PlayerInputManager instance;

    private List<GameObject> collidersInRange = new List<GameObject>();

    public Hint currentHint;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (collidersInRange.Count == 0 || !Input.anyKeyDown) return;

        GameObject nearestItem = collidersInRange[0];
        float nearestItemDistance = Vector3.Distance(transform.position, nearestItem.transform.position);
        for (int i = 0; i < collidersInRange.Count; i++)
        {
            float currentItemDistance = Vector3.Distance(transform.position, collidersInRange[i].transform.position);
            if (currentItemDistance > nearestItemDistance) break;
            nearestItemDistance = currentItemDistance;
            nearestItem = collidersInRange[i];
        }

        ExecuteActionsOfCollider(gameObject);
    }

    private void ExecuteActionsOfCollider(GameObject gameObject)
    {
        Interactable interactable = gameObject.GetComponent<Interactable>();
        interactable.Reach();
        if (interactable == null) return;
        interactable.Interact(KeyCode.Space);
    }

    private void OnTriggerEnter(Collider other)
    {
        collidersInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInRange.Remove(other.gameObject);
    }
}
