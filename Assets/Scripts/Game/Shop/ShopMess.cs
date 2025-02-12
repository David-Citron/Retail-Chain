using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShopMess : Interactable
{

    private bool isPlayerNear;
    private long spawnedAt;
    private bool isCleaning;

    void Start()
    {
        spawnedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear, gameObject => StartCleaning(),
            new Hint(Hint.GetHintButton(ActionType.Interaction) + " TO CLEAN", () => isPlayerNear && !isCleaning)));


        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
    }

    void Update() {}

    private void StartCleaning()
    {
        isCleaning = true;

        float cleaningTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - spawnedAt) / 2000f;
        cleaningTime = Mathf.Clamp(cleaningTime, 1, 6);

        CircleTimer.Start((int) cleaningTime);
        PlayerMovement.freeze = true;

        new ActionTimer(() => Input.GetKey(KeyCode.Space),
            () =>
            {
                CircleTimer.Stop();
                PlayerMovement.freeze = false;
                gameObject.SetActive(false);
                Destroy(gameObject);
                PlayerInputManager.instance.collidersInRange.Remove(gameObject);
            },
            () => {
                PlayerMovement.freeze = false;
                CircleTimer.Stop();
                isCleaning = false;
                UpdateHints();
            }, cleaningTime, 1).Run();
    }

    public override string GetTag() => "ShopMess";
    public override bool IsPlayerNear() => isPlayerNear;
    public override void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
}
