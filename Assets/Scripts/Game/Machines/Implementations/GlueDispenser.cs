using UnityEngine;

public class GlueDispenser : Machine
{
    private const int MAX_GLUE_AMOUNT = 50; // 50L is equal to 10x Glue Canister, one barell is 25L.

    private int glueAmount;

    public GlueDispenser() : base(MachineType.GlueDispenser) { }

    protected override void Update()
    {
        if (!isWithinTheRange) return;

        if (Input.GetKeyDown(KeyCode.Space)) return;

        var holdingItem = PlayerPickUp.Instance().GetValueOrDefault().holdingItem;

        if (holdingItem == null)
        {

            return;
        }
    }

    protected override void StartTimer()
    {
        actionTimer = new ActionTimer(
            () => ChangeMachineState(MachineState.Done),
            () => {
                actionTimer = null;
                ChangeMachineState(MachineState.Ready);
            }, currentRecipe.time, 1).Run();
    }

    protected override void PlaceItem(GameObject item, bool isInput)
    {
        if (item == null) return;

        item.SetActive(false);//Hide the object.
        item.transform.SetParent(transform); //Set parent the printer for the item
        item.transform.localPosition = Vector3.zero;
    }

    public override bool PlayAnimation() => true;
}
