using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private Dictionary<ObjType, AudioClip> Clips = new();
    private AudioSource musicAudio;
    private AudioSource soundAudio;

    public void Init()
    {
        Clips = new();
        musicAudio = GameObject.Find("MusicAudio").GetComponent<AudioSource>();
        soundAudio = GameObject.Find("SoundAudio").GetComponent<AudioSource>();
    }

    public void PlayMusic(ObjType type)
    {
        if (type == ObjType.None)
        {
            musicAudio.clip = null;
            musicAudio.Stop();
        }
        else
        {
            if(!Clips.ContainsKey(type))
                Clips[type] = ResManager.Instance.LoadResources<AudioClip>(type);
            musicAudio.clip = Clips[type];
            musicAudio.Play();
        }
    }
    public void PlaySound(ObjType type)
    {
        if (!Clips.ContainsKey(type))
            Clips[type] = ResManager.Instance.LoadResources<AudioClip>(type);
        soundAudio.PlayOneShot(Clips[type]);
        Debug.Log("PlaySound: " + type.ToString());
    }
}