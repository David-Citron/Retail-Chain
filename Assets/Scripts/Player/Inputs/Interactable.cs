using System;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{

    abstract List<Interaction> GetInteractions();

    public void Interact(KeyCode keyCode)
    {
        foreach (var interaction in GetInteractions())
        {
            if (interaction.keyCode != keyCode) continue;
        }
    }

    public void Reach()
    {
        foreach (var interaction in GetInteractions())
        {
            if (interaction.hints.Count == 0) continue;

            interaction.hints.ForEach(hint => HintSystem.EnqueueHint(hint));
        }
    }
}

public class Interaction
{
    public KeyCode keyCode { get; private set; }
    public Action onInteract {  get; private set; }
    public List<Hint> hints { get; } = new List<Hint>();

    public Interaction(KeyCode keyCode, Action onInteract, Hint[] hints)
    {
        this.keyCode = keyCode;
        this.onInteract = onInteract;
        this.hints.AddRange(hints);
    }
}
