using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class HintSystem : MonoBehaviour
{
    public static HintSystem instance;

    public TMP_SpriteAsset spriteAsset;

    public List<Hint> activeHints = new List<Hint>();
    public Queue<Hint> hints = new Queue<Hint>();

    private float passedTime;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (hints.Count <= 0) return;
        if (activeHints.Count >= 3) return;

        Hint hint = hints.Dequeue();
        activeHints.Add(hint);
        passedTime = 0;
        StartCoroutine(DisplayHint(hint));
    }

    public static void EnqueueHint(Hint hint)
    {
        if(instance.hints.Any(el => el.value.Equals(hint.value)) || instance.activeHints.Any(el => el.value.Equals(hint.value))) return; //Protection of possible duplicate hint.
        instance.hints.Enqueue(hint);
    }

    private IEnumerator DisplayHint(Hint hint)
    {
        CreateText(hint);

        if (hint.predicate != null)
        {
            while (hint.predicate.Invoke() && (hint.addiotionalPredicate?.Invoke() ?? true))
            {
                if (!hint.isActive)
                {
                    ResetText(hint);
                    yield break;
                }
                yield return null;
            }
        }
        else
        {
            while(hint.seconds > passedTime)
            {
                if (!hint.isActive)
                {
                    ResetText(hint);
                    yield break;
                }
                yield return new WaitForSecondsRealtime(.1f);
                passedTime += 0.1f;
            }
        }

        ResetText(hint);
    }

    private void CreateText(Hint hint)
    {
        hint.isActive = true;
        GameObject textObject = new GameObject("text-" + hint.value);

        textObject.transform.SetParent(transform);

        TMP_Text tmpText = textObject.AddComponent<TextMeshProUGUI>();

        tmpText.text = hint.value;
        tmpText.fontSize = 22; 
        tmpText.fontStyle = FontStyles.Bold;
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.spriteAsset = spriteAsset;

        hint.textObject = textObject;
    }

    private void ResetText(Hint hint)
    {
        Destroy(hint.textObject);   
        activeHints.Remove(hint);
        hint.isActive = false;
    }
}

public class Hint
{
    public GameObject textObject { get; set; }
    public string value { get; private set; }
    public float seconds { get; private set; }
    public Func<bool> predicate { get; set; }
    public Func<bool> addiotionalPredicate {  get; set; }
    public bool isActive { get; set; }


    public Hint(string value, float seconds, bool register, Func<bool> predicate = null)
    {
        this.value = value;
        this.seconds = seconds;
        this.predicate = predicate;

        if(register) HintSystem.EnqueueHint(this);
    }

    public Hint(string value, float seconds, Func<bool> predicate = null) : this(value, seconds, true, predicate) { }
    public Hint(string value, Func<bool> predicate = null) : this(value, 0, false, predicate) {}
    public static Hint Create(string value, float seconds)
    {
        return new Hint(value, seconds);
    }

    /// <summary>
    /// Hint stays while the condition is true.
    /// These hints are mostly usedin Update() method.
    /// </summary>
    /// <param name="value">The hint</param>
    /// <param name="predicate">The condition</param>
    /// <returns>Hint object</returns>
    public static Hint ShowWhile(string value, Func<bool> predicate)
    {
        return new Hint(value, 0, predicate);
    }
}

public static class HintText
{
    public static string GetHintButton(HintButton button) => $"<sprite={(int) button}>";
    
}

public enum HintButton
{
    A, B, C, D, E, F, G, H, I, J, K, L, M,
    N, O, P, Q, R, S, T, U, V, X, W, Y, Z,
    ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE,
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    MINUS, PLUS, TILDE, ASTERISK, SEMICOLON, SLASH, BRACKETS_LEFT, BRACKETS_RIGHT,
    QUOTATION, QUESTION_MARK, ALT, LESS_THAN, MORE_THAN, MOUSE, MOUSE_X, MOUSE_Y, MOUSE_XY,
    MOUSE_RIGHT, MOUSE_LEFT, MOUSE_MIDDLE, MOUSE_WHEEL, MOUSE_UP, MOUSE_DOWN, CURSOR,
    ENTER_ALT, ENTER, ENTER_TALL, PLUS_TALL, SHIFT_SUPER, SHIFT, BACKSPACE, CAPSLOCK,
    ESC, CTRL, END, PAGEDOWN, PAGEUP, NUMLOCK, DEL, SPACE, UP, DOWN, LEFT, RIGHT, PRTSCREEN,
    HOME, TAB, INSERT, BACKSPACE_ALT,
}