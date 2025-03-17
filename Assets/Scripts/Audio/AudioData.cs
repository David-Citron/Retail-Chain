using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioData
{
    [HideInInspector]
    public AudioSource source;
    public AudioClip clip;
    public AudioMixerGroup group;
    public AudioType type;

    public void Init(AudioSource sourceInstance)
    {
        source = sourceInstance;
        source.outputAudioMixerGroup = group;
        source.clip = clip;
    }
}

public enum AudioType
{
    None,
    Music, // Only 1 music can play at a time
    SoundEffect // Multiple sound effects can be active at one time
}