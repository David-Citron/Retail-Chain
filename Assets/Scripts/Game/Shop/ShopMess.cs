using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShopMess : Interactable
{

    private ActionTimer ratingDecrease;

    private bool isCleaning;
    private long spawnedAt;//When player stop cleaning new time will be saved.

    private int decreased;

    void Start()
    {
        AddInteraction(new Interaction(GetTag(), () => PressedKey(ActionType.Interaction) && !PlayerPickUp.IsHodlingItem() && isPlayerNear && isActiveAndEnabled, gameObject => StartCleaning(),
            new Hint(() => Hint.GetHintButton(ActionType.Interaction) + " TO CLEAN", () => !PlayerPickUp.IsHodlingItem() && isPlayerNear && !isCleaning)));

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

        PlayerPickUp.Instance().IfPresent(pikUp => pikUp.animator.SetBool("working", true));
        CircleTimer.Start(cleaningTime);

        new ActionTimer(() => HoldingKey(ActionType.Interaction),
            () => DestroyMess(),
            () => {
                CircleTimer.Stop();
                isCleaning = false;
                PlayerInputManager.isInteracting = false;
                UpdateHints();
                PlayerPickUp.Instance().IfPresent(pikUp => pikUp.animator.SetBool("working", false));
            }, cleaningTime, 1).Run();
    }

    public void ShowMess()
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        spawnedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        StartRatingDecreaseTimer();
    }

    private void StartRatingDecreaseTimer()
    {
        ratingDecrease = new ActionTimer(() =>
        {
            decreased++;
            ShopRating.instance.DecreaseRating(decreased * 0.05f);
            StartRatingDecreaseTimer();
        }, 15).Run();
    }

    private void DestroyMess()
    {
        ToggleIsPlayerNear();//When turning mess, the script is not enabled so that means we have to run this manually.
        if(ratingDecrease != null) ratingDecrease.Stop();
        gameObject.SetActive(false);
        isCleaning = false;
        PlayerInputManager.isInteracting = false;
        decreased = 0;
        PlayerPickUp.Instance().IfPresent(pikUp => pikUp.animator.SetBool("working", false));
    }

    public override string GetTag() => "ShopMess";
}
