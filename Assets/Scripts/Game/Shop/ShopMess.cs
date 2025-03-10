using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShopMess : Interactable
{

    private bool isCleaning;

    private long spawnedAt;//When player stop cleaning new time will be saved.

    void Start()
    {
        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && isPlayerNear && isActiveAndEnabled, gameObject => StartCleaning(),
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO CLEAN", () => isPlayerNear && !isCleaning)));

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
    }

    void Update() {}

    private void StartCleaning()
    {
        PlayerInputManager.isInteracting = true;
        isCleaning = true;

        float cleaningTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - spawnedAt) / 2000f;
        cleaningTime = (int) Mathf.Clamp(cleaningTime, 1, 5);

        CircleTimer.Start(cleaningTime);

        new ActionTimer(() => HoldingKey(ActionType.Interaction),
            () => DestroyMess(),
            () => {
                CircleTimer.Stop();
                isCleaning = false;
                PlayerInputManager.isInteracting = false;
                UpdateHints();
            }, cleaningTime, 1).Run();
    }

    public void ShowMess()
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        spawnedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    private void DestroyMess()
    {
        ToggleIsPlayerNear();//When turning mess, the script is not on so that means we have to run this manually.
        gameObject.SetActive(false);
        isCleaning = false;
        PlayerInputManager.isInteracting = false;
    }

    /// <summary>
    /// Checks if the mess is spawned more than 10 seconds.
    /// </summary>
    /// <returns>True if the shop is spawned for 10 seconds or more, otherwise false</returns>
    public bool IsSpawnedForWhile() => ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - spawnedAt) / 1000f) >= 10;

    public override string GetTag() => "ShopMess";
}
