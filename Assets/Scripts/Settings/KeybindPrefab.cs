using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class KeybindPrefab : MonoBehaviour
{
    public KeybindData keybindData;
    public GameObject primaryKey;
    public GameObject altKey;
    public KeyCode primaryKeyCode;
    public KeyCode altKeyCode;
    public TMP_Text label;

    // Start is called before the first frame update
    void Start()
    {
        ReloadIcons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadIcons()
    {
        if (KeybindManager.instance.spriteId.ContainsKey(primaryKeyCode))
        {
            primaryKey.GetComponentInChildren<TMP_Text>().text = "<sprite=" + KeybindManager.instance.spriteId[primaryKeyCode] + ">";
        }else
        {
            primaryKey.GetComponentInChildren<TMP_Text>().text = "";
        }
        if (KeybindManager.instance.spriteId.ContainsKey(altKeyCode))
        {
            altKey.GetComponentInChildren<TMP_Text>().text = "<sprite=" + KeybindManager.instance.spriteId[altKeyCode] + ">";
        }
        else
        {
            altKey.GetComponentInChildren<TMP_Text>().text = "";
        }
    }

    public void ChangeKeybind(bool settingAlt)
    {
        Settings.keybindChangeOverlay.SetActive(true);
        StartCoroutine(ReadKeyPress(settingAlt));
    }

    private IEnumerator ReadKeyPress(bool settingAlt)
    {
        bool set = false;
        while (!set)
        {
            yield return new WaitForEndOfFrame();
            //if (!Input.anyKeyDown) continue;
            if (Input.GetKey(KeyCode.Mouse0)) set = true;
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKey(key)) continue;
                if (!KeybindManager.instance.spriteId.ContainsKey(key)) continue;
                set = true;
                switch (keybindData.influence)
                {
                    case KeybindInfluence.Positive:
                        if (!settingAlt)
                        {
                            // KeybindManager.keybinds[keybindData.action].positiveKey = key;
                            primaryKeyCode = key;
                        }
                        else
                        {
                            // KeybindManager.keybinds[keybindData.action].positiveAltKey = key;
                            altKeyCode = key;
                        }
                        break;
                    case KeybindInfluence.Negative:
                        if (!settingAlt)
                        {
                            // KeybindManager.keybinds[keybindData.action].negativeKey = key;
                            primaryKeyCode = key;
                        }
                        else
                        {
                            // KeybindManager.keybinds[keybindData.action].negativeAltKey = key;
                            altKeyCode = key;
                        }
                        break;
                    default:
                        Debug.LogError("Keybind influence is not set");
                        break;
                }
            }
        }
        ReloadIcons();
        Settings.keybindChangeOverlay.SetActive(false);
    }
}
[Serializable]
public enum KeybindInfluence
{
    None,
    Positive,
    Negative,
}
