using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public List<AudioData> audioData = new List<AudioData>();

    private ActionTimer musicTimer = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void Start()
    {
        MusicCycle();
    }

    private void Initialize()
    {
        audioData.ForEach(data =>
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            data.Init(source);
        });
    }

    public void Play(int index)
    {
        AudioData data = audioData[index];
        if (data == null) return;
        if (data.type == AudioType.Music)
        { 
            audioData.ForEach(a => {
                if (a.type != AudioType.Music || !a.source.isPlaying) return;
                a.source.Stop();
            });
        }
        data.source.Play();
    }

    private void MusicCycle()
    {
        foreach (AudioData data in audioData)
        {
            if (!data.source.isPlaying || data.type != AudioType.Music) continue;
            ResetMusicTimer();
            return;
        }
        List<AudioData> musicData = new List<AudioData>();
        foreach (AudioData data in audioData)
        {
            if (data.type != AudioType.Music) continue;
            musicData.Add(data);
        }
        int randomIndex = Random.Range(0, musicData.Count);
        audioData[audioData.IndexOf(musicData[randomIndex])].source.Play();
        ResetMusicTimer();
    }

    private void ResetMusicTimer()
    {
        musicTimer = new ActionTimer(() => MusicCycle(), Random.Range(20, 60)).Run(false);
    }
}
