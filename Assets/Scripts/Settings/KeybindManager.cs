using System.Collections.Generic;
using UnityEngine;

public class KeybindManager : MonoBehaviour
{

    public static Dictionary<ActionType, ActionKeybind> keybinds = new Dictionary<ActionType, ActionKeybind>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    void Start() {}
    void Update() {}


    public static void Initialize()
    {
        keybinds.Add(ActionType.HorizontalInput, new ActionKeybind(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow, 100));
        keybinds.Add(ActionType.VerticalInput, new ActionKeybind(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow, 100));


        keybinds.Add(ActionType.Interaction, new ActionKeybind(KeyCode.Space));
        keybinds.Add(ActionType.PickUpItem, new ActionKeybind(KeyCode.E));
        keybinds.Add(ActionType.DropItem, new ActionKeybind(KeyCode.Q));
        keybinds.Add(ActionType.OpenMenu, new ActionKeybind(KeyCode.E));
    }
}