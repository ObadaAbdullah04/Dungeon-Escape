using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_AudioManager : MonoBehaviour
{
    public static MM_AudioManager Instance { get; private set; }

    [Header("Sound Clips")]
    public AudioClip startClip;
    public AudioClip QuitClip;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayStartSound()
    {
        PlayClip(startClip);
    }

    public void PlayQuitSound()
    {
        PlayClip(QuitClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
