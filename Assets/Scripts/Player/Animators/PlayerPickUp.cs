using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{

    private static PlayerPickUp instance;

    public static GameObject holdingItem;

    [SerializeField] private GameObject playerHands;
    public Animator animator;

    void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();

        Interactable.interactions.AddRange(GetInteractions());
    }

    void Update() {}

    public void PickUp(GameObject itemGameObject)
    {
        if (itemGameObject == null || holdingItem != null || itemGameObject.tag == null || !itemGameObject.tag.StartsWith("Item")) return;

        holdingItem = itemGameObject;
        UpdateHandsPosition();
        animator.SetBool("holding", true);

        itemGameObject.SetActive(true); //To be sure, activate the gameobject.

        UpdateRigidbody(itemGameObject, true);

        itemGameObject.transform.SetParent(playerHands.transform);
        itemGameObject.transform.localPosition = Vector3.zero;
        itemGameObject.transform.localRotation = Quaternion.Euler(-90, 0, -90);

        PlayerInputManager.instance.RemoveCollider(holdingItem);
        PlayerInputManager.CustomInteractionHints(GetInteractions(), holdingItem);
    }

    public void DropHoldingItem()
    {
        if (holdingItem == null) return;

        animator.SetBool("holding", false);

        holdingItem.transform.SetParent(null);
        UpdateRigidbody(holdingItem, false);

        holdingItem = null;
    }

    private void UpdateRigidbody(GameObject item, bool pickedUp)
    {  
        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        if (rigidbody != null)  rigidbody.isKinematic = pickedUp;

        foreach (var collider in item.GetComponents<Collider>())
        {
            collider.enabled = !pickedUp;
        }
    }

    private void UpdateHandsPosition()
    {
        ItemManager.GetItemType(holdingItem).IfPresent(item =>
        {
            switch (item)
            {
                case ItemType.GlueBarrel:
                    playerHands.transform.localPosition = new Vector3(-0.55f, 0.2f, 0);
                    break;
                case ItemType.Wood:
                    playerHands.transform.localPosition = new Vector3(-0.55f, 0.45f, 0);
                    break;
                case ItemType.Package:
                    playerHands.transform.localPosition = new Vector3(-0.53f, 0.45f, 0);
                    break;
                default:
                    playerHands.transform.localPosition = new Vector3(-0.45f, 0.45f, 0);
                    break;
            }
        });
    }

    public static bool IsHodlingItem() => holdingItem != null;
    public static ItemType GetHoldingType() => ItemManager.GetItemType(holdingItem).GetValueOrDefault();
    public static Optional<PlayerPickUp> Instance() => Optional<PlayerPickUp>.Of(instance);
    public static List<Interaction> GetInteractions() => new List<Interaction>() {
        new Interaction(() => Interactable.PressedKey(ActionType.PickUpItem), gameObject => instance.PickUp(gameObject), new Hint(() => Hint.GetHintButton(ActionType.PickUpItem) + " TO PICK UP", () => holdingItem == null)),
        new Interaction(() => Interactable.PressedKey(ActionType.DropItem), gameObject => instance.DropHoldingItem(), new Hint(() => Hint.GetHintButton(ActionType.DropItem) + " TO DROP", () => holdingItem != null))
    };
    
}