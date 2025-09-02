using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager inst;
    public int sourceCount = 5;

    private AudioClip music;
    private AudioClip ambience;
    private AudioSource musicSource;
    private AudioSource ambienceSource;

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
        ambience = Sounds.inst.ambience;
        
    }

    private void Update()
    {
        if (music != null)
        {
            if (musicSource.volume != Sounds.inst.musicVolume)
            {
                musicSource.volume = Sounds.inst.musicVolume;
            }
        }

        if (ambience != null)
        {
            if (ambienceSource.volume != Sounds.inst.ambienceVolume)
            {
                ambienceSource.volume = Sounds.inst.ambienceVolume;
            }
        }
    }

    public void PlayMusic(float volume = 1)
    {
        if (music == null) return;

        var source = GetAvailableAudioSource();
        musicSource = source;
        musicSource.pitch = 1;
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void PlayAmbience(float volume = 1)
    {
        if (ambience == null) return;

        var source = GetAvailableAudioSource();
        ambienceSource = source;
        ambienceSource.clip = ambience;
        ambienceSource.pitch = 1;
        ambienceSource.loop = true;
        ambienceSource.volume = volume;
        ambienceSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null || !musicSource.isPlaying) return;

        musicSource.Stop();
    }

    public void StopAmbience(float volume = 1)
    {
        if (ambienceSource == null || !ambienceSource.isPlaying) return;

        ambienceSource.Stop();
    }

    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if (clip == null) return;
        float maxPitch = 1.15f;
        float minPitch = 0.85f;

        var source = GetAvailableAudioSource();
        source.loop = false;
        source.pitch = Random.Range(minPitch, maxPitch);
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
