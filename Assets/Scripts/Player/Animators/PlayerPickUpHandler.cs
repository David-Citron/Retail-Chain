using UnityEngine;

public class PlayerPickUpHandler: MonoBehaviour
{
    // The point where picked items will be held
    public Transform holdPoint;

    // Layer mask for items the player can pick up
    public LayerMask pickupLayer;

    // Current item being held
    private GameObject heldItem;

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                // Attempt to pick up an item
                TryPickupItem();
            }
            else
            {
                // Drop the currently held item
                DropItem();
            }
        }
    }

    private void TryPickupItem()
    {
        // Raycast to find an item in front of the player
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.Log("Called");

        if (Physics.Raycast(ray, out hit, 2f, pickupLayer))
        {
            // Check if the hit object has a rigidbody
            if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                // Set the held item
                heldItem = hit.collider.gameObject;

                // Disable physics and attach it to the hold point
                rb.isKinematic = true;
                heldItem.transform.position = holdPoint.position;
                heldItem.transform.parent = holdPoint;
            }
        }
    }

    private void DropItem()
    {
        if (heldItem != null)
        {
            // Detach and re-enable physics
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            heldItem.transform.parent = null;

            // Optional: Add a small force to the item
            rb.AddForce(transform.forward * 2f, ForceMode.Impulse);

            // Clear the held item
            heldItem = null;
        }
    }
}
