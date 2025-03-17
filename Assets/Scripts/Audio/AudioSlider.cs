using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public AudioMixerGroup group;
    public string controlName;
    public Slider slider;

    public void OnValueChange(float value)
    {
        group.audioMixer.SetFloat(controlName, value);
    }

    private void Start()
    {
        slider.minValue = -80f;
        slider.maxValue = 0f;
        float currentVolume = 0;
        group.audioMixer.GetFloat(controlName, out currentVolume);
        slider.value = currentVolume;
        slider.onValueChanged.AddListener(OnValueChange);
    }
}
