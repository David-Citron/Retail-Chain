using System.Collections;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{

    private static PlayerPickUp instance;

    [SerializeField] private GameObject playerHands;
    public GameObject holdingItem;
    public Animator animator;

    void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void PickUp(GameObject item)
    {
        StartCoroutine(PickUpItem(item));
    }

    private IEnumerator PickUpItem(GameObject itemGameObject)
    {
        if (holdingItem != null) yield break;
        holdingItem = itemGameObject;
        animator.SetBool("holding", true);

        //if (currentHint != null) currentHint.stop = true;
        yield return new WaitForSecondsRealtime(.3f);

        //if (!Machine.IsReachable) currentHint = Hint.ShowWhile(HintText.GetHintButton(HintButton.Q) + " TO DROP", () => holdingItem != null && !Machine.IsReachable);

        UpdateHandsPosition();

        itemGameObject.transform.SetParent(playerHands.transform);
        itemGameObject.transform.localPosition = Vector3.zero;
        itemGameObject.transform.localRotation = Quaternion.Euler(-90, 0, -90);

        itemGameObject.SetActive(true); //To be sure, activate the gameobject.

        UpdateRigidbody(itemGameObject, true);
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
        foreach (var collider in item.GetComponents<Collider>())
        {
            if (collider.isTrigger) continue;
            collider.enabled = !pickedUp;
        }

        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        if (rigidbody == null) return;
        rigidbody.isKinematic = pickedUp;
    }

    private void UpdateHandsPosition()
    {
        Item.GetItemType(holdingItem).IfPresent(item =>
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

    public static Optional<PlayerPickUp> Instance() => Optional<PlayerPickUp>.Of(instance);
}