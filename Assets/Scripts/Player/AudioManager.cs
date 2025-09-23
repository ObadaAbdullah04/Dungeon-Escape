using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Player Sound Clips")]
    public AudioClip playerAttackClip;
    public AudioClip playerJumpClip;
    public AudioClip playerHitClip;
    public AudioClip playerDieClip;
    public AudioClip playerCollectClip;

    private AudioSource _audioSource;

    private void Awake()
    {
        // Singleton Protection
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

    private void PlayClip(AudioClip clip, float volume=0.7f)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayPlayerAttack() => PlayClip(playerAttackClip);
    public void PlayPlayerJump() => PlayClip(playerJumpClip);
    public void PlayPlayerHit() => PlayClip(playerHitClip);
    public void PlayPlayerDie() => PlayClip(playerDieClip);
    public void PlayPlayerCollect() => PlayClip(playerCollectClip, 0.5f);
}
