using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public static List<Interaction> interactions;

    public static void AddInteraction(Interaction interaction) => interactions.Add(interaction);
    public static bool PressedKey(ActionType actionType, bool ignoreCheck)
    {
        if (!ignoreCheck && MenuManager.instance.IsAnyOpened() || PlayerInputManager.isInteracting) return false;
        var keys = KeybindManager.instance.keybinds[actionType];
        return Input.GetKeyDown(keys.positiveKey) || Input.GetKeyDown(keys.positiveAltKey);
    }

    public static bool PressedKey(ActionType actionType) => PressedKey(actionType, false);

    public static bool HoldingKey(ActionType actionType)
    {
        if (MenuManager.instance.IsAnyOpened()) return false;
        var keys = KeybindManager.instance.keybinds[actionType];
        return Input.GetKey(keys.positiveKey) || Input.GetKey(keys.positiveAltKey);
    }

    public List<Interaction> GetCurrentInteractions(string tag) => interactions.FindAll(interaction => interaction.tag != null && interaction.tag.Equals(GetTag()));

    public bool isPlayerNear;

    public void ToggleIsPlayerNear() => isPlayerNear = !isPlayerNear;
    
    public void UpdateHints()
    {
        if (!isPlayerNear) return;
        
        foreach (var interaction in GetCurrentInteractions(GetTag()))
        {
            interaction.hints.ForEach(hint => {
                if (hint.isActive || hint.predicate == null || !hint.predicate.Invoke()) return;
                HintSystem.EnqueueHint(hint);
            });
        }
    }
    public abstract string GetTag();
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

    public Interaction(string tag, Func<bool> prediction, Action<GameObject> onInteract, Hint hint) : this(tag, prediction, onInteract, new Hint[] {hint}) {}
    public Interaction(Func<bool> prediction, Action<GameObject> onInteract, Hint hint) : this(null, prediction, onInteract, new Hint[] { hint }) {}
    public Interaction(Func<bool> prediction, Action<GameObject> onInteract) : this(null, prediction, onInteract, new Hint[] {}) {}
}

[Serializable]
public enum ActionType
{
    None,
    HorizontalInput,
    VerticalInput,
    PickUpItem,
    DropItem,
    OpenMenu,
    Interaction,
    Help
}