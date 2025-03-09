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

    [SerializeField] private GameObject hintContent;

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
                    DestroyHint(hint);
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
                    DestroyHint(hint);
                    yield break;
                }
                yield return new WaitForSecondsRealtime(.1f);
                passedTime += 0.1f;
            }
        }

        DestroyHint(hint);
    }

    private void CreateText(Hint hint)
    {
        hint.isActive = true;
        GameObject textObject = new GameObject("text-" + hint.value);

        textObject.transform.SetParent(hintContent.transform);
        textObject.transform.localPosition = Vector3.zero;
        textObject.transform.localScale = Vector3.one;
        textObject.transform.SetSiblingIndex(0);

        TMP_Text tmpText = textObject.AddComponent<TextMeshProUGUI>();

        tmpText.text = hint.value;
        tmpText.enableAutoSizing = true;
        tmpText.color = hint.color;
        tmpText.fontSizeMin = 22;
        tmpText.fontSizeMax = 26;
        tmpText.fontStyle = FontStyles.Normal;
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.spriteAsset = spriteAsset;

        hint.textObject = textObject;
    }

    private void DestroyHint(Hint hint)
    {
        Destroy(hint.textObject);   
        activeHints.Remove(hint);
        hint.isActive = false;
    }
}

public class Hint
{
    public GameObject textObject { get; set; }
    public Color color { get; set; }
    public string value { get; private set; }
    public float seconds { get; private set; }
    public Func<bool> predicate { get; set; }
    public Func<bool> addiotionalPredicate {  get; set; }
    public bool isActive { get; set; }


    public Hint(string value, Color color, float seconds, bool register, Func<bool> predicate = null)
    {
        this.value = value;
        this.color = color;
        this.seconds = seconds;
        this.predicate = predicate;

        if(register) HintSystem.EnqueueHint(this);
    }

    public Hint(string value, float seconds, bool register, Func<bool> predicate = null) : this(value, Color.white, seconds, register, predicate) { }
    public Hint(string value, float seconds, Func<bool> predicate = null) : this(value, seconds, true, predicate) { }
    public Hint(string value, Color color, float seconds, Func<bool> predicate = null) : this(value, color, seconds, true, predicate) { }
    public Hint(string value, Func<bool> predicate = null) : this(value, 0, false, predicate) { }

    public static Hint Create(string value, float seconds) => new Hint(value, seconds);
    public static Hint Create(string value, Color color, float seconds) => new Hint(value, color, seconds);


    /// <summary>
    /// Hint stays while the condition is true.
    /// These hints are mostly usedin Update() method.
    /// </summary>
    /// <param name="value">The hint</param>
    /// <param name="predicate">The condition</param>
    /// <returns>Hint object</returns>
    public static Hint ShowWhile(string value, Func<bool> predicate) => new Hint(value, 0, predicate);
    public static string GetHintButton(ActionType actionType)
    {
        var positiveKey = KeybindManager.instance.keybinds[actionType].positiveKey;
        var spriteId = KeybindManager.instance.spriteId[positiveKey];

        Debug.LogError("Positive key: " + positiveKey + "; sprite Id: " + spriteId);

        return $"<sprite={spriteId}>";
    } 
}