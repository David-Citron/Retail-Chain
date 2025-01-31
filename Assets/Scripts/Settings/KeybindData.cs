using UnityEngine;

[CreateAssetMenu(fileName = "KeybindData", menuName = "ScriptableObjects/KeybindData", order = 2)]
public class KeybindData : ScriptableObject
{
    public string label = "Missing";
    public ActionType action = ActionType.None;
    public KeybindInfluence influence = KeybindInfluence.None;
    public KeyCode defaultPrimaryKey = KeyCode.None;
    public KeyCode defaultAlternativeKey = KeyCode.None;
}
