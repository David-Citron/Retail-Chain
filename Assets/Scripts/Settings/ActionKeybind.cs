using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionKeybind
{
    public KeyCode positiveKey;
    public KeyCode positiveAltKey;
    public KeyCode negativeKey;
    public KeyCode negativeAltKey;
    public float axis { get; private set; } = 0;
    public float sensitivity = 1;

    const float DEFAULT_SENSITIVITY = 1;

    public ActionKeybind(KeyCode positive) : this(positive, KeyCode.None, KeyCode.None, KeyCode.None, DEFAULT_SENSITIVITY) { }
    public ActionKeybind(KeyCode positive, KeyCode positiveAlt) : this(positive, positiveAlt, KeyCode.None, KeyCode.None, DEFAULT_SENSITIVITY) { }
    public ActionKeybind(KeyCode positive, KeyCode positiveAlt, KeyCode negative, KeyCode negativeAlt) : this(positive, positiveAlt, negative, negativeAlt, DEFAULT_SENSITIVITY) { }

    public ActionKeybind(KeyCode positive, float sensitivity) : this(positive, KeyCode.None, KeyCode.None, KeyCode.None, sensitivity) { }
    public ActionKeybind(KeyCode positive, KeyCode positiveAlt, float sensitivity) : this(positive, positiveAlt, KeyCode.None, KeyCode.None, sensitivity) { }
    public ActionKeybind(KeyCode positive, KeyCode positiveAlt, KeyCode negative, KeyCode negativeAlt, float sensitivity)
    {
        positiveKey = positive;
        positiveAltKey = positiveAlt;
        negativeKey = negative;
        negativeAltKey = negativeAlt;
        this.sensitivity = sensitivity;
        axis = 0;
    }

    public bool GetInput()
    {
        return Input.GetKeyDown(positiveKey) || (positiveAltKey != KeyCode.None && Input.GetKeyDown(positiveAltKey));
    }

    public float CalculateAxis()
    {
        float current = 0;

        bool positivePress = KeyPressed(positiveKey, positiveAltKey);
        bool negativePress = KeyPressed(negativeKey, negativeAltKey);

        if (positivePress && negativePress)
        {
            if (axis < 0)
            {
                current = axis + sensitivity * Time.deltaTime;
                axis = current;
                axis = Mathf.Clamp(axis, 0, 1);
            }
            else if (axis > 0)
            {
                current = axis - sensitivity * Time.deltaTime;
                axis = current;
                axis = Mathf.Clamp(axis, -1, 0);
            }
            Debug.Log(axis);
            return axis;
        }

        if (positivePress)
        {
            axis = Mathf.Clamp(axis, 0, 1);
            current = axis + sensitivity * Time.deltaTime;
        }
        else if (negativePress)
        {
            axis = Mathf.Clamp(axis, -1, 0);
            current = axis - sensitivity * Time.deltaTime;
        }
        current = Mathf.Clamp(current, -1, 1);
        axis = current;
        Debug.Log(axis);
        return axis;
    }

    private bool KeyPressed(KeyCode key, KeyCode alt)
    {
        return (key != KeyCode.None && Input.GetKey(key)) || (alt != KeyCode.None && Input.GetKey(alt));
    }
}
