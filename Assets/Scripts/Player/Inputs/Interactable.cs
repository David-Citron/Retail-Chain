using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public static List<Interaction> interactions = new List<Interaction>();
    public abstract string GetTag();

    public void AddInteraction(Interaction interaction) => interactions.Add(interaction);

    public void UpdateHints()
    {
        foreach (var interaction in interactions)
        {
            interaction.hints.ForEach(hint => {
                if (hint.predicate != null && !hint.predicate.Invoke() || hint.addiotionalPredicate != null && !hint.addiotionalPredicate()) return;
                HintSystem.EnqueueHint(hint);
            });
        }
    }
}

public class Interaction
{
    public Func<bool> prediction { get; private set; }
    public Action<GameObject> onInteract {  get; private set; }
    public List<Hint> hints { get; } = new List<Hint>();

    public Interaction(Func<bool> prediction, Action<GameObject> onInteract, Hint[] hints)
    {
        this.prediction = prediction;
        this.onInteract = onInteract;
        this.hints.AddRange(hints);
    }

    public Interaction(Func<bool> prediction, Action<GameObject> onInteract, Hint hint) : this(prediction, onInteract, new Hint[] {hint}) {}
}
