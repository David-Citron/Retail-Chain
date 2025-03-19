using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class DisplayTable : Interactable
{

    [SerializeField] private List<GameObject> inputSlots;
    [SerializeField] public List<Transform> customerPoints;
    private List<GameObject> currentItems;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(collider.size.x + 0.25f, 1.8f, collider.size.z + 0.25f);
    }


    void Start()
    {
        currentItems = new List<GameObject>();

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && GetNearestSlot().isInValidDistance, gameObject => PutItem(PlayerPickUp.holdingItem), 
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO ADD ITEM", () => PlayerPickUp.IsHodlingItem() && GetNearestSlot().isInValidDistance)));


        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear && GetNearestSlot().isInValidDistance, gameObject => PickUp(),
            new Hint(() => Hint.GetHintButton(ActionType.PickUpItem) + " TO PICK UP", () => !PlayerPickUp.IsHodlingItem() && GetNearestSlot().IsReadyToPickUp())));
    }

    void Update()
    {
        InputInfo inputInfo = GetNearestSlot();
        if (inputInfo == null || !inputInfo.isInValidDistance) return;
        UpdateHints();
    }

    private void PickUp()
    {

        if (PlayerPickUp.IsHodlingItem())
        {
            Hint.Create("DROP CURRENT ITEM", 2);
            return;
        }

        InputInfo nearestInput = GetNearestSlot();
        if (nearestInput == null || !nearestInput.IsReadyToPickUp()) return;


        if (currentItems.Count <= 0) return;
        var item = nearestInput.inputPlace.gameObject.transform.GetChild(0).gameObject;
        if (item == null) return;
        PlayerPickUp.Instance().IfPresent(handler => handler.PickUp(item));
        currentItems.Remove(item);
    }

    private void PutItem(GameObject item)
    {
        if(item == null) return;
        InputInfo input = GetNearestSlot();
        if (inputSlots.Count == 0 || input == null || !input.IsValid()) return;

        PlayerPickUp.Instance().IfPresent(handler =>
        {
            handler.DropHoldingItem();
            currentItems.Add(item);
            PlaceItem(input != null ? input.inputPlace : null, item);
        });
    }

    private void PlaceItem(GameObject place, GameObject item)
    {
        if (item == null) return;

        item.transform.SetParent(place.transform);

        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        if (rigidbody == null) return;
        rigidbody.isKinematic = true;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    internal InputInfo GetNearestSlot()
    {
        if (!isPlayerNear || inputSlots.Count == 0) return new InputInfo(); //If the inputs are 0.

        GameObject nearestSlot = inputSlots[0];
        float nearestItemDistance = Vector3.Distance(PlayerInputManager.instance.transform.position, nearestSlot.transform.position);

        for (int i = 0; i < inputSlots.Count; i++)
        {
            float currentItemDistance = Vector3.Distance(PlayerInputManager.instance.transform.position, inputSlots[i].transform.position);
            if (currentItemDistance > nearestItemDistance) break;
            nearestItemDistance = currentItemDistance;
            nearestSlot = inputSlots[i];
        }

        if (nearestSlot.transform.childCount != 0)
        {
            Hint.ShowWhile(() => "ITEM SLOT IS FULL", () => PlayerPickUp.GetHoldingType() != ItemType.None && GetNearestSlot().IsReadyToPickUp());
        }

        return new InputInfo(nearestSlot, nearestItemDistance <= 1.15f, nearestSlot.transform.childCount != 0);
    }

    public GameObject FindClosestItemSlot(Vector3 positionOrigin)
    {
        GameObject result = null;
        if (inputSlots.Count == 0)
        {
            Debug.LogWarning("There are no input slots in the DisplayTable!");
            return result;
        }

        result = inputSlots[0];
        float nearestDistance = Vector3.Distance(positionOrigin, inputSlots[0].transform.position);
        for (int i = 1; i < inputSlots.Count; i++)
        {
            float currentDistance = Vector3.Distance(positionOrigin, inputSlots[i].transform.position);
            if (currentDistance > nearestDistance) break;
            nearestDistance = currentDistance;
            result = inputSlots[i];
        }

        return result;
    }

    public bool RemoveItemFromSlot(GameObject slot)
    {
        Transform item = slot.transform.GetChild(0);
        if (item == null)
            return false;
        if (item.gameObject == null)
            return false;
        Destroy(item.gameObject);
        return true;
    }

    public Item GetItemFromSlot(GameObject slot)
    {
        Item item = null;
        item = slot.GetComponentInChildren<Item>();
        if (item == null)
            return null;
        return item;
    }

    public List<GameObject> GetSlots() => inputSlots;
    public List<GameObject> GetCurrentItems() => currentItems;
    public override string GetTag() => "DisplaySlot";
}
