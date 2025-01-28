using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class DisplaySlot : Interactable
{

    [SerializeField] private List<GameObject> inputSlots = new List<GameObject>();

    private bool isPlayerNear;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }


    void Start()
    {
        AddInteraction(new Interaction(GetTag(), () => Input.GetKeyDown(KeyCode.Space) && isPlayerNear, gameObject => AddItem(), 
            new Hint(Hint.GetHintButton(HintButton.SPACE) + " TO ADD ITEM", () => isPlayerNear && GetNearestSlot().isInValidDistance)));
    }

    void Update()
    {
        
    }

    public void AddItem()
    {

    }

    internal InputInfo GetNearestSlot()
    {
        if (!isPlayerNear || inputSlots.Count == 0) return null; //If the inputs are 0.

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

        return new InputInfo(nearestSlot, nearestItemDistance <= 1.3f, nearestSlot.transform.childCount != 0);
    }

    public override string GetTag() => "DisplaySlot";
    public override bool IsPlayerNear() => isPlayerNear;

    public override void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
}
