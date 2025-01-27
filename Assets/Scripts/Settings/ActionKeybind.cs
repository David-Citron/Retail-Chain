using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionKeybind : MonoBehaviour
{
    public KeyCode positiveKey;
    public KeyCode positiveAltKey;
    public KeyCode negativeKey;
    public KeyCode negativeAltKey;
    public float axis = 0;
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

    public float ReadAxis()
    {
        return axis;
    }

    public bool ReadInput()
    {
        return axis > 0;
    }

    private void Update()
    {
        float current = 0;
        if (Input.GetKey(positiveKey) || (positiveAltKey != KeyCode.None && Input.GetKey(positiveAltKey)))
        {
            current = axis + sensitivity * Time.deltaTime;
        }else
        {
            if ((negativeKey != KeyCode.None && Input.GetKey(negativeKey)) || (negativeAltKey != KeyCode.None && Input.GetKey(negativeAltKey)))
            {
                current = axis - sensitivity * Time.deltaTime;
            }
        }
        if ((negativeKey != KeyCode.None && Input.GetKey(negativeKey)) || (negativeAltKey != KeyCode.None && Input.GetKey(negativeAltKey)))
        {
            current = axis - sensitivity * Time.deltaTime;
        }
        else
        {
            if (Input.GetKey(positiveKey) || (positiveAltKey != KeyCode.None && Input.GetKey(positiveAltKey)))
            {
                current = axis + sensitivity * Time.deltaTime;
            }
        }
        current = Mathf.Clamp(current, -1, 1);
        axis = current;
        Debug.Log(axis);
    }
}
