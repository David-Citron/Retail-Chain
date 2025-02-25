using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Printer : Machine
{
    public TMP_Text timeDisplay;
    public RawImage doneIcon;

    public Printer() : base(MachineType.Printer) {}

    protected override void OnStart()
    {
        timeDisplay.text = "WAITING";
        doneIcon.gameObject.SetActive(false);

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.PickUpItem) && isPlayerNear, collider => PickUp(),
            new Hint(Hint.GetHintButton(ActionType.PickUpItem) + " TO PICK UP", () => PlayerPickUp.GetHoldingType() == ItemType.None && (machineState == MachineState.Done || machineState == MachineState.Ready))
        ));

        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear, collider => StartInteraction(), new Hint[] {
            new Hint("PRINTING..", () => machineState == MachineState.Working),
            new Hint(Hint.GetHintButton(ActionType.Interaction) + " TO PRINT", () => IsValid(PlayerPickUp.holdingItem) && machineState != MachineState.Done),
            new Hint("INVALID ITEM", () => !IsValid(PlayerPickUp.holdingItem) && machineState == MachineState.Idling)
        }));
    }

    protected override void Update() {}

    protected override void ChangeMachineState(MachineState newState)
    {
        base.ChangeMachineState(newState);
        doneIcon.gameObject.SetActive(newState == MachineState.Done);
        timeDisplay.gameObject.SetActive(newState != MachineState.Done);
        timeDisplay.text = newState == MachineState.Idling ? "WAITING" : ConvertPassedTime(0);
    }

    protected override void StartTimer()
    {
        actionTimer = new ActionTimer(action => timeDisplay.text = ConvertPassedTime(action.passedTime),
            () => ChangeMachineState(MachineState.Done),
            () => {
                actionTimer = null;
                ChangeMachineState(MachineState.Ready);
            }, currentRecipe.time, 1).Run();
    }

    protected override void PlaceItem(GameObject place, GameObject item) {
        item.SetActive(false);//Hide the object.
        item.transform.SetParent(transform); //Set parent the printer for the item
        item.transform.localPosition = Vector3.zero;
    }

    private string ConvertPassedTime(float passedTime)
    {
        if(currentRecipe == null) return "";
        
        int time = currentRecipe.time - (int) passedTime;
        int minutes = 0;
        while(time > 60)
        {
            minutes++;
            time -= 60;
        }
        return (minutes > 9 ? minutes : "0" + minutes) + ":" + (time > 9 ? time : "0" + time);
    }

    protected override void StartInteraction()
    {
        if (machineState != MachineState.Ready) PutItem(PlayerPickUp.holdingItem);
        if (currentRecipe == null || actionTimer != null || machineState != MachineState.Ready || CooldownHandler.IsUnderCreateIfNot(machineType + "_working", 1)) return;

        StartTimer();
        ChangeMachineState(MachineState.Working);
    }

    protected override bool IsValid(GameObject item) => ItemManager.GetItemType(item).GetValueOrDefault() == ItemType.EmptyBook;
    public override bool PlayAnimation() => false;
    public override string GetTag() => "MachinePrinter";
    public override Vector3 GetColliderSize(BoxCollider collider) => new Vector3(collider.size.x, collider.size.y, 1.5f);
}
