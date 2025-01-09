using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public List<GameObject> collidersInRange = new List<GameObject>();

    public Hint currentHint;

    private void Start() { instance = this; }

    private void Update()
    {
        if (!Input.anyKeyDown) return;
        KeyCode pressedKey = KeyCode.None;
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKeyDown(keyCode)) continue;
            pressedKey = keyCode;
        }

        if (collidersInRange.Count == 0)
        {
            if (PlayerPickUp.holdingItem != null) CustomInteraction(PlayerPickUp.PickUpInteractions(), PlayerPickUp.holdingItem, pressedKey);
            return;
        }


        GameObject nearestItem = collidersInRange[0];
        float nearestItemDistance = Vector3.Distance(transform.position, nearestItem.transform.position);
        for (int i = 0; i < collidersInRange.Count; i++)
        {
            float currentItemDistance = Vector3.Distance(transform.position, collidersInRange[i].transform.position);
            if (currentItemDistance > nearestItemDistance) break;
            nearestItemDistance = currentItemDistance;
            nearestItem = collidersInRange[i];
        }

        Interactable interactable = nearestItem.GetComponent<Interactable>();
        if (interactable == null)
        {
            ProcessPlayerInteractions(nearestItem, pressedKey);
            return;
        }

        interactable.Interact(pressedKey, nearestItem);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(PlayerPickUp.holdingItem)) return; //Ensure that the holding item is not in collider.
        collidersInRange.Add(other.gameObject);
        UpdateHints(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInRange.Remove(other.gameObject);
        UpdateHints(other.gameObject, false);
    }

    private void UpdateHints(GameObject collidedObject, bool entry)
    {
        Interactable interactable = collidedObject.GetComponent<Interactable>();
        if (interactable == null)
        {
            if (!entry) return;
            if (isItem(collidedObject)) CustomInteractionHints(PlayerPickUp.PickUpInteractions(), collidedObject);
            return;
        }

        interactable.inReach = !interactable.inReach;
        interactable.UpdateHints(collidedObject);
    }

    private void ProcessPlayerInteractions(GameObject gameObject, KeyCode keyCode)
    {
        if (isItem(gameObject)) CustomInteraction(PlayerPickUp.PickUpInteractions(), gameObject, keyCode);

    }

    private void CustomInteraction(List<Interaction> interactions, GameObject gameObject, KeyCode keyCode)
    {
        foreach (var interaction in interactions)
        {
            if (interaction.keyCode != keyCode) continue;
            interaction.onInteract.Invoke(gameObject);
        }
    }

    public static void CustomInteractionHints(List<Interaction> interactions, GameObject gameObject)
    {
        foreach (var interaction in interactions)
        {
            interaction.hints.ForEach(hint =>
            {
                hint.addiotionalPredicate = () =>
                {
                    var list = instance.collidersInRange;
                    return PlayerPickUp.holdingItem != null || list.Contains(gameObject);
                };
                if (hint.predicate == null || !hint.predicate.Invoke()) return;
                HintSystem.EnqueueHint(hint);
            });
        }
    }

    private bool isItem(GameObject gameObject) => gameObject.tag != null && gameObject.tag.StartsWith("Item");
}
