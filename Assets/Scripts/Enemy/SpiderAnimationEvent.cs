using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAnimationEvent : MonoBehaviour
{
    private Spider _spider;

    private AudioSource _audioSource;
    [Header("Sound Clips")]
    public AudioClip attackClip;
    public AudioClip deathClip;

    private void Start()
    {
        _spider = transform.parent.GetComponent<Spider>();

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void Fire()
    {
        _spider.Attack();
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

    public void PlayDieSound()
    {
        DieSound();
    }

    public void AttackSound() => PlayClip(attackClip);
    public void DieSound() => PlayClip(deathClip);
}
