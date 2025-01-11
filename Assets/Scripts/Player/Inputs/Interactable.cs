using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public static List<Interaction> interactions = new List<Interaction>();
    public abstract string GetTag();
    public abstract bool IsPlayerNear();

    public abstract void ToggleIsPlayerNear();

    public void AddInteraction(Interaction interaction) => interactions.Add(interaction);

    public List<Interaction> GetCurrentInteractions(string tag) => interactions.FindAll(interaction => interaction.tag != null && interaction.tag.Equals(GetTag()));

    public void UpdateHints()
    {
        if (!IsPlayerNear()) return;
        
        foreach (var interaction in GetCurrentInteractions(GetTag()))
        {
            interaction.hints.ForEach(hint => {
                bool isActive = hint.isActive;
                bool exists = hint.predicate == null;
                bool predicate = hint.predicate.Invoke();
                if (isActive || exists || !predicate) return;
                
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

       // foreach (var hint in hints)
       // {
       //     hint.addiotionalPredicate = () => PlayerInputManager.IsIn(tag);
       // }
    }

    public Interaction(string tag, Func<bool> prediction, Action<GameObject> onInteract, Hint hint) : this(tag, prediction, onInteract, new Hint[] {hint}) { }
    public Interaction(Func<bool> prediction, Action<GameObject> onInteract, Hint hint) : this(null, prediction, onInteract, new Hint[] { hint }) { }
}
