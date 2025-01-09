using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{

    private static PlayerPickUp instance;

    [SerializeField] private GameObject playerHands;
    public static GameObject holdingItem;
    public Animator animator;

    void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();

        Interactable.interactions.AddRange(GetInteractions());
    }

    void Update() {}

    public void PickUp(GameObject item) => StartCoroutine(PickUpItem(item));
    private IEnumerator PickUpItem(GameObject itemGameObject)
    {
        if (itemGameObject == null || holdingItem != null) yield break;
        holdingItem = itemGameObject;
        animator.SetBool("holding", true);

        yield return new WaitForSecondsRealtime(.3f);

        UpdateHandsPosition();

        itemGameObject.transform.SetParent(playerHands.transform);
        itemGameObject.transform.localPosition = Vector3.zero;
        itemGameObject.transform.localRotation = Quaternion.Euler(-90, 0, -90);

        itemGameObject.SetActive(true); //To be sure, activate the gameobject.

        UpdateRigidbody(itemGameObject, true);
        PlayerInputManager.instance.collidersInRange.Remove(holdingItem);
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

    public static ItemType GetHoldingType() => ItemManager.GetItemType(holdingItem).GetValueOrDefault();
    public static Optional<PlayerPickUp> Instance() => Optional<PlayerPickUp>.Of(instance);
    public static List<Interaction> GetInteractions() => new List<Interaction>() {
        new Interaction(() => Input.GetKeyDown(KeyCode.E), gameObject => instance.PickUp(gameObject), new Hint(HintText.GetHintButton(HintButton.E) + " TO PICK UP", () => holdingItem == null)),
        new Interaction(() => Input.GetKeyDown(KeyCode.Q), gameObject => instance.DropHoldingItem(), new Hint(HintText.GetHintButton(HintButton.Q) + " TO DROP", () => holdingItem != null))
    };
    
}