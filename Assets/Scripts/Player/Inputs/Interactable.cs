using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    protected List<Interaction> interactions = new List<Interaction>();
    public bool inReach { get; set; }
    public abstract string GetTag();

    public void AddInteraction(Interaction interaction) => interactions.Add(interaction);

    public void Interact(KeyCode keyCode, GameObject gameObject)
    {
        if (GetTag().Equals("Item") && gameObject.tag.StartsWith(GetTag()) || !gameObject.CompareTag(GetTag())) return;

        foreach (var interaction in interactions)
        {
            if (interaction.keyCode != keyCode) continue;
            interaction.onInteract.Invoke(gameObject);
            Debug.Log("-1");
        }
    }

    public void UpdateHints(GameObject gameObject)
    {
        if (GetTag().Equals("Item") && gameObject.tag.StartsWith(GetTag()) || !gameObject.CompareTag(GetTag())) return;

        UpdateHints();
    }

    public void UpdateHints()
    {
        foreach (var interaction in interactions)
        {
            if (interaction.hints.Count == 0) continue;

            interaction.hints.ForEach(hint => {
                hint.addiotionalPredicate = () => inReach;
                if (hint.predicate == null || !hint.predicate.Invoke()) return;
                HintSystem.EnqueueHint(hint);
            });
        }
    }
}

public class Interaction
{
    public KeyCode keyCode { get; private set; }
    public Action<GameObject> onInteract {  get; private set; }
    public List<Hint> hints { get; } = new List<Hint>();

    public Interaction(KeyCode keyCode, Action<GameObject> onInteract, Hint[] hints)
    {
        this.keyCode = keyCode;
        this.onInteract = onInteract;
        this.hints.AddRange(hints);
    }
}
