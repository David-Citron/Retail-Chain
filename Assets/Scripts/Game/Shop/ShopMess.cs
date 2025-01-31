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
        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Cleaning) && isPlayerNear, gameObject => StartCleaning(),
            new Hint(Hint.GetHintButton(HintButton.SPACE) + " TO CLEAN", () => isPlayerNear && !isCleaning)));


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

        new ActionTimer(() => Input.GetKey(KeyCode.Space),
            () =>
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                PlayerInputManager.instance.collidersInRange.Remove(gameObject);
            },
            () => {
                isCleaning = false;
                UpdateHints();
            }, cleaningTime, 1).Run();
    }

    public override string GetTag() => "ShopMess";
    public override bool IsPlayerNear() => isPlayerNear;
    public override void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
}
