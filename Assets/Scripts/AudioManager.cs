using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip, float volume = 0.2f)
    {
        musicSource.PlayOneShot(clip, volume);
    }

    public void PlaySfx(AudioClip clip, float volume = 0.7f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
