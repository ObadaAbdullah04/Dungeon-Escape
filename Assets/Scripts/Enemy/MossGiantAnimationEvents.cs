using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossGiantAnimationEvents : MonoBehaviour
{
    private AudioSource _audioSource;
    [Header("Sound Clips")]
    public AudioClip attackClip;
    public AudioClip hitClip;
    public AudioClip deathClip;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    public void PlayAttackSound()
    {
        AttackSound();
    }    
    public void PlayHitSound()
    {
        HitSound();
    }    
    public void PlayDieSound()
    {
        DieSound();
    }

    public void AttackSound() => PlayClip(attackClip);
    public void HitSound() => PlayClip(hitClip);
    public void DieSound() => PlayClip(deathClip);
}
