using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public static bool isInteracting;

    private List<GameObject> collidersInRange;

    private void Start() {
        instance = this;
        isInteracting = false;
        collidersInRange = new List<GameObject>();
    }

    private void Update()
    {
        if (!Input.anyKeyDown) return;

        List<GameObject> colliders = GetColliders();
        GameObject nearestItem = null;
        if (colliders.Count > 0)
        {
            nearestItem = colliders[0];
            if (nearestItem == null) return;

            float nearestItemDistance = Vector3.Distance(transform.position, nearestItem.transform.position);
            for (int i = 0; i < colliders.Count; i++)
            {
                float currentItemDistance = Vector3.Distance(transform.position, colliders[i].transform.position);
                if (currentItemDistance > nearestItemDistance) break;
                nearestItemDistance = currentItemDistance;
                nearestItem = colliders[i];
            }
        }

        if (Interactable.interactions == null) return;
        Interactable.interactions.ForEach(interaction => {
            if (!interaction.prediction.Invoke()) return;
            interaction.onInteract.Invoke(nearestItem);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(PlayerPickUp.holdingItem)) return; //Ensure that the holding item is not in collider.
        GetColliders().Add(other.gameObject);
        UpdateInteractable(other.gameObject);
        UpdateHints(other.gameObject, true);
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!GetColliders().Contains(other.gameObject) || other.gameObject == null) return;

        GetColliders().Remove(other.gameObject);
        UpdateInteractable(other.gameObject);
        //UpdateHints(other.gameObject, false);
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
                    var list = new List<GameObject>(instance.GetColliders());
                    return PlayerPickUp.holdingItem != null || list.Contains(gameObject);
                };
                if (hint.predicate == null || !hint.predicate.Invoke()) return;
                HintSystem.EnqueueHint(hint);
            });
        }
    }

    public void UpdateCurrentHints()
    {
        foreach (var item in collidersInRange)
        {
            Interactable interactable = item.GetComponent<Interactable>();
            if(interactable == null) continue;
            interactable.UpdateHints();
        }
    }

    private void UpdateInteractable(GameObject collider)
    {
       Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable == null) return;
        interactable.ToggleIsPlayerNear();

        if (interactable.isPlayerNear) return;
        interactable.GetCurrentInteractions(interactable.GetTag()).ForEach(interaction => interaction.hints.ForEach(hint => hint.isActive = false));
    }

    private bool IsItem(GameObject gameObject) => gameObject.tag != null && gameObject.tag.StartsWith("Item");
    public void RemoveCollider(GameObject gameObject) => collidersInRange.Remove(gameObject);
    public List<GameObject> GetColliders()
    {
        collidersInRange.RemoveAll(item => item == null);
        return collidersInRange;
    }
}
