using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public static List<Interaction> interactions = new List<Interaction>();
    public List<Interaction> GetCurrentInteractions(string tag) => interactions.FindAll(interaction => interaction.tag != null && interaction.tag.Equals(GetTag()));

    public abstract string GetTag();
    public abstract bool IsPlayerNear();
    public abstract void ToggleIsPlayerNear();

    protected void AddInteraction(Interaction interaction) => interactions.Add(interaction);
    protected bool PressedKey(ActionType actionType) => Input.GetKeyDown(KeybindManager.keybinds[actionType].positiveKey);
    protected bool HoldingKey(ActionType actionType) => Input.GetKey(KeybindManager.keybinds[actionType].positiveKey);

    public void UpdateHints()
    {
        if (!IsPlayerNear()) return;
        
        foreach (var interaction in GetCurrentInteractions(GetTag()))
        {
            interaction.hints.ForEach(hint => {
                if (hint.isActive || hint.predicate == null || !hint.predicate.Invoke()) return;
                HintSystem.EnqueueHint(hint);
            });
        }
    }
}

public class Interaction
{
    public readonly string tag;

    public Func<bool> prediction { get; private set; }
    public Action<GameObject> onInteract {  get; private set; }
    public List<Hint> hints { get; } = new List<Hint>();

    public Interaction(string tag, Func<bool> prediction, Action<GameObject> onInteract, Hint[] hints)
    {
        this.tag = tag;
        this.prediction = prediction;
        this.onInteract = onInteract;
        this.hints.AddRange(hints);
    }

    public Interaction(string tag, Func<bool> prediction, Action<GameObject> onInteract, Hint hint) : this(tag, prediction, onInteract, new Hint[] {hint}) { }
    public Interaction(Func<bool> prediction, Action<GameObject> onInteract, Hint hint) : this(null, prediction, onInteract, new Hint[] { hint }) { }
}

public enum ActionType
{
    None,
    HorizontalInput,
    VerticalInput,
    PickUpItem,
    DropItem,
    OpenMenu,
    Interaction,
    Cleaning


}