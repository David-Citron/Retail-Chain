using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeybindPrefab : MonoBehaviour
{
    public KeybindData keybindData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeKeybind(bool settingAlt)
    {
        Settings.keybindChangeMenu.SetActive(true);
        StartCoroutine(ReadKeyPress(settingAlt));
    }

    public IEnumerator ReadKeyPress(bool settingAlt)
    {
        bool set = false;
        while (!set)
        {
            yield return new WaitForEndOfFrame();
            if (!Input.anyKeyDown) continue;
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    set = true;
                    switch (keybindData.influence)
                    {
                        case KeybindInfluence.Positive:
                            if (!settingAlt)
                            {
                                KeybindManager.keybinds[keybindData.action].positiveKey = key;
                            }
                            else
                            {
                                KeybindManager.keybinds[keybindData.action].positiveAltKey = key;
                            }
                            break;
                        case KeybindInfluence.Negative:
                            if (!settingAlt)
                            {
                                KeybindManager.keybinds[keybindData.action].negativeKey = key;
                            }
                            else
                            {
                                KeybindManager.keybinds[keybindData.action].negativeAltKey = key;
                            }
                            break;
                        default:
                            Debug.LogError("Keybind type is not set");
                            break;
                    }
                }
            }
        }
    }
}

public enum KeybindInfluence
{
    None,
    Positive,
    Negative,
}
