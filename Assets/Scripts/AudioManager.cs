using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager inst;
    public int sourceCount = 5;

    private AudioClip music;
    private AudioSource musicSource;

    private List<AudioSource> sources = new List<AudioSource>();


    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        for (int i = 0; i < sourceCount; i++)
        {
            sources.Add(CreateAudioSource());
        }
    }

    private void Start()
    {
        music = Sounds.inst.music;
        
    }

    public void PlayMusic(float volume = 1)
    {
        if (music == null) return;

        var source = GetAvailableAudioSource();
        musicSource = source;
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null || !musicSource.isPlaying) return;

        musicSource.Stop();
    }

    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if (clip == null) return;
        var source = GetAvailableAudioSource();
        source.loop = false;
        source.PlayOneShot(clip, volume);
    }


    private AudioSource CreateAudioSource()
    {
        GameObject o = new GameObject("AudioSource", typeof(AudioSource));
        o.transform.SetParent(transform);
        return o.GetComponent<AudioSource>();
    }

    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = null;

        for (int i = 0; i < sources.Count; i++)
        {
            if (!sources[i].isPlaying)
            {
                source = sources[i];
                break;
            }
        }

        if (source == null)
        {
            source = CreateAudioSource();
            sources.Add(source);
        }
        return source;
    }
}
