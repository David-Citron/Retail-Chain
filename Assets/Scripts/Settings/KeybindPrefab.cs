using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeybindPrefab : MonoBehaviour
{
    public ActionKeybind keybind;
    public KeyType keyType;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeKeybind()
    {
        Settings.keybindChangeMenu.SetActive(true);
        StartCoroutine(ReadKeyPress());
    }

    // maybe add param like primary/alt?
    public IEnumerator ReadKeyPress()
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
                    // TODO
                }
            }
        }
    }
}

public enum KeyType
{
    None,
    Positive,
    Negative,
}
