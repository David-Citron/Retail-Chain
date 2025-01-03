using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerPickUp : MonoBehaviour
{

    private static PlayerPickUp instance;

    private List<GameObject> itemsInRange = new List<GameObject>();

    [SerializeField] private GameObject playerHands;

    public Animator animator;
    public GameObject holdingItem;


    public Hint currentHint;

    private void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();
        itemsInRange.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (holdingItem != null || itemsInRange.Count == 0) return;
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
        Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
        if ((rigidbody != null && rigidbody.isKinematic) || MachineManager.IsGameObjectInMachine(other.gameObject)) return;
        itemsInRange.Add(other.gameObject);
        if (holdingItem == null && !Machine.IsReachable)
        {
            if (currentHint != null) currentHint.stop = true;
            currentHint = Hint.ShowWhile(HintText.GetHintButton(HintButton.E) + " TO PICK UP", () => holdingItem == null && itemsInRange.Count > 0 && !Machine.IsReachable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        itemsInRange.Remove(other.gameObject);
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

        if (currentHint != null) currentHint.stop = true;
        yield return new WaitForSecondsRealtime(.3f);

        if(!Machine.IsReachable) currentHint = Hint.ShowWhile(HintText.GetHintButton(HintButton.Q) + " TO DROP", () => holdingItem != null && !Machine.IsReachable);

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
   
    public static Optional<Animator> PlayerAnimator()
    {
        Animator animator = null;
        Instance().IfPresent(instance => animator = instance.animator);
        return Optional<Animator>.Of(animator);
    }
}
