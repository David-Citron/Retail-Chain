using System.Collections.Generic;
using UnityEngine;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager instance = null;

    public Dictionary<ActionType, ActionKeybind> keybinds = new Dictionary<ActionType, ActionKeybind>();

    // List of supported bindable keys:
    public Dictionary<KeyCode, int> spriteId = new Dictionary<KeyCode, int>();

    [Tooltip("Related axis KeybindData must be passed in a row")]
    public List<KeybindData> keybindDataList = new List<KeybindData>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    void Start() {}
    void Update() {}


    public void Initialize()
    {
        LoadDefaultsFromKeybindData();
        /*keybinds.Add(ActionType.HorizontalInput, new ActionKeybind(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow, 100));
        keybinds.Add(ActionType.VerticalInput, new ActionKeybind(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow, 100));

        keybinds.Add(ActionType.Interaction, new ActionKeybind(KeyCode.Space));
        keybinds.Add(ActionType.PickUpItem, new ActionKeybind(KeyCode.E));
        keybinds.Add(ActionType.DropItem, new ActionKeybind(KeyCode.Q));
        keybinds.Add(ActionType.OpenMenu, new ActionKeybind(KeyCode.Escape));*/

        SetSupportedKeys();
    }

    public void LoadDefaultsFromKeybindData()
    {
        // Load defaults from keybind data list
        // Make sure to pass both axis data behind each other
        keybinds.Clear();

        for (int i = 0; i < keybindDataList.Count; i++) 
        {
            ActionType action = keybindDataList[i].action;
            if ((i + 1) < keybindDataList.Count && keybindDataList[i + 1].action == action)
            {
                keybinds.Add(action, ActionKeybindFromData(keybindDataList[i], keybindDataList[i + 1], 100));
                i++;
                continue;
            }
            KeybindData keybindData = keybindDataList[i];
            keybinds.Add(action, new ActionKeybind(keybindData.defaultPrimaryKey, keybindData.defaultAlternativeKey));
        }
    }

    private static ActionKeybind ActionKeybindFromData(KeybindData first, KeybindData second, float sensitivity)
    {
        List<KeybindData> dataList = new List<KeybindData>() { first, second };
        KeyCode positive = KeyCode.None;
        KeyCode positiveAlt = KeyCode.None;
        KeyCode negative = KeyCode.None;
        KeyCode negativeAlt = KeyCode.None;
        dataList.ForEach(data =>
        {
            switch (data.influence)
            {
                case KeybindInfluence.Positive:
                    positive = data.defaultPrimaryKey;
                    positiveAlt = data.defaultAlternativeKey;
                    break;
                case KeybindInfluence.Negative:
                    negative = data.defaultPrimaryKey;
                    negativeAlt = data.defaultAlternativeKey;
                    break;
                default:
                    Debug.LogError("Keybind influence not set in KeybindData");
                    break;
            }
        });
        return new ActionKeybind(positive, positiveAlt, negative, negativeAlt, sensitivity);
    }

    private void SetSupportedKeys()
    {
        spriteId = new Dictionary<KeyCode, int>(){
            { KeyCode.A, 0 },
            { KeyCode.B, 1 },
            { KeyCode.C, 2 },
            { KeyCode.D, 3 },
            { KeyCode.E, 4 },
            { KeyCode.F, 5 },
            { KeyCode.G, 6 },
            { KeyCode.H, 7 },
            { KeyCode.I, 8 },
            { KeyCode.J, 9 },
            { KeyCode.K, 10 },
            { KeyCode.L, 11 },
            { KeyCode.M, 12 },
            { KeyCode.N, 13 },
            { KeyCode.O, 14 },
            { KeyCode.P, 15 },
            { KeyCode.Q, 16 },
            { KeyCode.R, 17 },
            { KeyCode.S, 18 },
            { KeyCode.T, 19 },
            { KeyCode.U, 20 },
            { KeyCode.V, 21 },
            { KeyCode.X, 22 },
            { KeyCode.W, 23 },
            { KeyCode.Y, 24 },
            { KeyCode.Z, 25 },
            { KeyCode.Alpha0, 26 },
            { KeyCode.Alpha1, 27 },
            { KeyCode.Alpha2, 28 },
            { KeyCode.Alpha3, 29 },
            { KeyCode.Alpha4, 30 },
            { KeyCode.Alpha5, 31 },
            { KeyCode.Alpha6, 32 },
            { KeyCode.Alpha7, 33 },
            { KeyCode.Alpha8, 34 },
            { KeyCode.Alpha9, 35 },
            { KeyCode.F1, 36 },
            { KeyCode.F2, 37 },
            { KeyCode.F3, 38 },
            { KeyCode.F4, 39 },
            { KeyCode.F5, 40 },
            { KeyCode.F6, 41 },
            { KeyCode.F7, 42 },
            { KeyCode.F8, 43 },
            { KeyCode.F9, 44 },
            { KeyCode.F10, 45 },
            { KeyCode.F11, 46 },
            { KeyCode.F12, 47 },
            { KeyCode.Return, 83 },
            { KeyCode.Space, 88 },
            { KeyCode.Escape, 90 },
            { KeyCode.UpArrow, 96 },
            { KeyCode.DownArrow, 97 },
            { KeyCode.Tab, 99 },
            { KeyCode.LeftArrow, 100 },
            { KeyCode.RightArrow, 101 },
            { KeyCode.LeftShift, 79 },
            { KeyCode.LeftControl, 89 },
        };
    }
}