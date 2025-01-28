
using System.Collections.Generic;
using UnityEngine;

public class KeybindManager : MonoBehaviour
{

    private static Dictionary<ActionType, ActionKeybind> keybinds = new Dictionary<ActionType, ActionKeybind>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Initialize();
    }
    void Update() {}


    public static void Initialize()
    {
        Update(ActionType.HorizontalInput, new ActionKeybind(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow, 100));
        Update(ActionType.VerticalInput, new ActionKeybind(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow, 100));



        Update(ActionType.MachineInteraction, new ActionKeybind(KeyCode.Space));
        Update(ActionType.PickUpItem, new ActionKeybind(KeyCode.E));
        Update(ActionType.DropItem, new ActionKeybind(KeyCode.Q));
        Update(ActionType.OpenMenu, new ActionKeybind(KeyCode.E));

        Update(ActionType.Cleaning, new ActionKeybind(KeyCode.Space));

    }

    public static void Update(ActionType action, ActionKeybind keybind)
    {
        if(Contains(action)) {
            keybinds[action] = keybind;
            return;
        }

        keybinds.Add(action, keybind);
    }

    public static ActionKeybind GetKeybind(ActionType action) => Contains(action) ? keybinds[action] : null;
    public static bool Contains(ActionType action) => keybinds.ContainsKey(action);

}
