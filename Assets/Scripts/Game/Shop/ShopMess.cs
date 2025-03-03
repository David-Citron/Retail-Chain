using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShopMess : Interactable
{
    private Interaction interaction;

    private bool isPlayerNear;
    private long spawnedAt;
    private bool isCleaning;

    void Start()
    {
        spawnedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        interaction = new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear, gameObject => StartCleaning(),
            new Hint(Hint.GetHintButton(ActionType.Interaction) + " TO CLEAN", () => isPlayerNear && !isCleaning));

        AddInteraction(interaction);

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
    }

    void Update() {}

    private void StartCleaning()
    {
        isCleaning = true;

        float cleaningTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - spawnedAt) / 2000f;
        cleaningTime = (int) Mathf.Clamp(cleaningTime, 1, 6);

        CircleTimer.Start(cleaningTime);

        new ActionTimer(() => Input.GetKey(KeyCode.Space),
            () => DestroyMess(),
            () => {
                CircleTimer.Stop();
                isCleaning = false;
                UpdateHints();
            }, cleaningTime, 1).Run();
    }

    private void DestroyMess()
    {
        gameObject.SetActive(false);
        interactions.Remove(interaction);

        if (this == null) return;

        PlayerInputManager.instance.RemoveCollider(gameObject);
        Destroy(gameObject);
    }

    public override string GetTag() => "ShopMess";
    public override bool IsPlayerNear() => isPlayerNear;
    public override void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
}
