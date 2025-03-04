using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;
    public Sound[] musics;

    private float soundVol;
    private bool soundON;
    private float musicVol;
    private bool musicON;

    public float SoundVol
    {
        get => soundVol;
        set { soundVol = value; }
    }

    public bool SoundON
    {
        get => soundON;
        set { soundON = value; }
    }

    public bool MusicON
    {
        get => musicON;
        set => musicON = value;
    }

    public float MusicVol
    {
        get => musicVol;
        set => musicVol = value;
    }

    public void Construct()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void PlayMusic(string music)
    {
        if (!musicON)
        {
            return;
        }

        Sound s = Array.Find(musics, item => item.name == music);
        s.source.volume = soundVol;
        s.source.Play();
    }

    public void PlaySound(string sound)
    {
        if (!soundON)
        {
            return;
        }

        Sound s = Array.Find(sounds, item => item.name == sound);
        s.source.volume = soundVol;
        s.source.Play();
    }

    public void StopSound(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        s.source.Stop();
    }

    public void PlayButtonClick()
    {
        PlaySound("ButtonClick");
    }
}