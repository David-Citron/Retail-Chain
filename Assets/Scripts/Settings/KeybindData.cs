using UnityEngine;

[CreateAssetMenu(fileName = "KeybindData", menuName = "ScriptableObjects/KeybindData", order = 2)]
public class KeybindData : ScriptableObject
{
    [SerializeField] public string label;
    [SerializeField] public ActionType action;
    [SerializeField] public KeybindInfluence influence;
    [SerializeField] public KeyCode defaultPrimaryKey;
    [SerializeField] public KeyCode defaultAlternativeKey;
}
