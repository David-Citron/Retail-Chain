using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class DisplaySlot : Interactable
{

    [SerializeField] private List<GameObject> inputSlots;
    private List<GameObject> currentItems;

    private bool isPlayerNear;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(collider.size.x, 1.8f, collider.size.z);
    }


    void Start()
    {
        currentItems = new List<GameObject>();

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear, gameObject => PutItem(PlayerPickUp.holdingItem), 
            new Hint[] {
                new Hint(Hint.GetHintButton(KeyCode.Space) + " TO ADD ITEM", () => PlayerPickUp.IsHodlingItem() && GetNearestSlot().isInValidDistance),
                new Hint("YOU NEED TO HOLD AN ITEM", () => !PlayerPickUp.IsHodlingItem() && isPlayerNear)
            }));
    }

    void Update()
    {
        InputInfo inputInfo = GetNearestSlot();
        if (inputInfo == null || !inputInfo.isInValidDistance) return;
        UpdateHints();
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

    public void AddItem()
    {

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
            Hint.ShowWhile("ITEM SLOT IS FULL", () => PlayerPickUp.GetHoldingType() != ItemType.None && GetNearestSlot().IsReadyToPickUp());
        }

        return new InputInfo(nearestSlot, nearestItemDistance <= 1f, nearestSlot.transform.childCount != 0);
    }

    public override string GetTag() => "DisplaySlot";
    public override bool IsPlayerNear() => isPlayerNear;
    public override void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
}
