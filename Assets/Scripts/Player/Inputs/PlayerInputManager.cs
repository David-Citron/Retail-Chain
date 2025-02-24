using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public List<GameObject> collidersInRange;

    private void Start() {
        instance = this;
        collidersInRange = new List<GameObject>();
    }

    private void Update()
    {
        if (!Input.anyKeyDown) return;

        GameObject nearestItem = null;
        if (collidersInRange.Count > 0)
        {
            nearestItem = collidersInRange[0];
            if (nearestItem == null) return;

            float nearestItemDistance = Vector3.Distance(transform.position, nearestItem.transform.position);
            for (int i = 0; i < collidersInRange.Count; i++)
            {
                float currentItemDistance = Vector3.Distance(transform.position, collidersInRange[i].transform.position);
                if (currentItemDistance > nearestItemDistance) break;
                nearestItemDistance = currentItemDistance;
                nearestItem = collidersInRange[i];
            }
        }

        Interactable.interactions.ForEach(interaction => {
            if (!interaction.prediction.Invoke()) return;
            interaction.onInteract.Invoke(nearestItem);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(PlayerPickUp.holdingItem)) return; //Ensure that the holding item is not in collider.
        collidersInRange.Add(other.gameObject);
        UpdateInteractable(other.gameObject);
        UpdateHints(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInRange.Remove(other.gameObject);
        UpdateInteractable(other.gameObject);
    }

    private void UpdateHints(GameObject collidedObject, bool entry)
    {
        Interactable interactable = collidedObject.GetComponent<Interactable>();
        if (interactable == null)
        {
            if (!entry) return;
            if (IsItem(collidedObject)) CustomInteractionHints(PlayerPickUp.GetInteractions(), collidedObject);
            return;
        }

        interactable.UpdateHints();
    }

    public static void CustomInteractionHints(List<Interaction> interactions, GameObject gameObject)
    {
        foreach (var interaction in interactions)
        {
            interaction.hints.ForEach(hint =>
            {
                hint.addiotionalPredicate = () =>
                {
                    var list = new List<GameObject>(instance.collidersInRange);
                    return PlayerPickUp.holdingItem != null || list.Contains(gameObject);
                };
                if (hint.predicate == null || !hint.predicate.Invoke()) return;
                HintSystem.EnqueueHint(hint);
            });
        }
    }

    private void UpdateInteractable(GameObject collider)
    {
       Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable == null) return;
        interactable.ToggleIsPlayerNear();

        if (interactable.IsPlayerNear()) return;
        interactable.GetCurrentInteractions(interactable.GetTag()).ForEach(interaction => interaction.hints.ForEach(hint => hint.isActive = false));
    }

    private bool IsItem(GameObject gameObject) => gameObject.tag != null && gameObject.tag.StartsWith("Item");
}
