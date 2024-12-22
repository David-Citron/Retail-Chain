using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class HintSystem : MonoBehaviour
{
    public static HintSystem instance;

    public static Queue<Hint> hints = new Queue<Hint>();

    public Hint hint;

    private TMP_Text text;
    private static bool isHintActive = false;

    private float passedTime;

    void Start()
    {
        instance = this;
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (!isHintActive && hints.Count > 0)
        {
            Hint hint = hints.Dequeue();
            this.hint = hint;
            passedTime = 0;
            StartCoroutine(DisplayHint());
        }
    }

    public static void EnqueueHint(Hint hint)
    {
        hints.Enqueue(hint);
    }

    public static bool IsActive() => isHintActive;

    private IEnumerator DisplayHint()
    {
        isHintActive = true;
        text.text = hint.Value;

        if (hint.Predicate != null)
        {
            while (hint.Predicate.Invoke())
            {
                if (hint.Stop)
                {
                    Reset();
                    yield break;
                }
                yield return null;
            }
        }
        else
        {
            while(hint.Seconds > passedTime)
            {
                if (hint.Stop)
                {
                    Reset();
                    yield break;
                }
                yield return new WaitForSecondsRealtime(.2f);
                passedTime += 0.2f;
            }
        }

        Reset();
    }

    private void Reset()
    {
        text.text = "";
        isHintActive = false;
        hint = null;
    }

}

public class Hint
{
    public string Value { get; private set; }
    public float Seconds { get; private set; }
    public Func<bool> Predicate { get; private set; }
    public bool Stop { get; set; }

    private Hint(string value, float seconds, Func<bool> predicate = null)
    {
        Value = value;
        Seconds = seconds;
        Predicate = predicate;

        HintSystem.EnqueueHint(this);
    }

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
    public static string GetHintButton(HintButton button)
    {
        return $"<sprite={(int) button}>";
    }
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