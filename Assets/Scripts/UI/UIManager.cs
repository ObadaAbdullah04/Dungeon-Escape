using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance==null)
            {
                Debug.LogError("UI Manager is NULL!");
            }
            return _instance;
        }
    }

    private AudioSource _audioSource;
    [Header("Sound Clips")]
    public AudioClip Button1Clip;
    public AudioClip Button2Clip;
    public AudioClip selectionClip;
    public AudioClip PurchaseClip;

    private void Awake()
    {
        _instance = this;
    }

    public Image selectionImage;
    public Image[] healthBars;

    public Text playerGemCount;
    public Text UI_GemCountText;

    [Header("Mobile Controls")]
    public FixedJoystick joystick;
    public Button jumpButton;
    public Button attackButton;

    public event Action OnJumpPressed;
    public event Action OnAttackPressed;

    [Header("Panels")]
    public GameObject loseGamePanel;
    public GameObject GameOnPanel;
    public bool isStarted = false;

    [SerializeField] private GameObject mobileUI;

    private void Start()
    {

#if !UNITY_EDITOR
    if (!Application.isMobilePlatform)
    {
        mobileUI.SetActive(false);
    }
#endif

        GameOnPanel.SetActive(true);
        loseGamePanel.SetActive(false);
        selectionImage.gameObject.SetActive(false);

        if (!isStarted)
        {
            Time.timeScale = 0;
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public float GetJoystickHorizontal() => joystick.Horizontal;

    public void OpenShop(int gemCount)
    {
        playerGemCount.text = gemCount.ToString()+" G";
    }

    public void UpdateShopSelection(int yPos)
    {
        selectionImage.gameObject.SetActive(true);
        selectionImage.rectTransform.anchoredPosition = new Vector2(selectionImage.rectTransform.anchoredPosition.x, yPos);
        PlaySelectionSound();
    }
    public void UpdateGemCount(int count)
    {
        UI_GemCountText.text = count.ToString();
    }

    public void UpdateLives(int livesRemaining)
    {
        for (int i = 0; i < healthBars.Length; i++)
        {
            // Only enable the hearts below the current lives
            healthBars[i].enabled = i < livesRemaining;
        }
    }

    public void TriggerJump()
    {
        OnJumpPressed?.Invoke();
    }    
    public void TriggeAttack()
    {
        OnAttackPressed?.Invoke();
    }

    public void GameOn()
    {
        PlayButton1Sound();
        GameOnPanel.SetActive(false);
        Time.timeScale = 1;
        isStarted = true;
    }

    public void LoseGame()
    {
        loseGamePanel.SetActive(true);
    }

    public void ResetGame()
    {
        PlayButton1Sound();
        StartCoroutine(DelayThenLoad(Button1Clip));
    }

    public void QuitButton()
    {
        PlayButton2Sound();
        StartCoroutine(DelayQuit(Button2Clip));
    }

    IEnumerator DelayThenLoad(AudioClip audioClip)
    {
        isStarted = true;
        yield return new WaitForSeconds(audioClip.length);
        SceneManager.LoadScene("Game");
        Debug.Log("Reset!");
    }

    IEnumerator DelayQuit(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
        Debug.Log("Quit!");
        Application.Quit();
    }
    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    public void PlayButton1Sound()
    {
        Button1Sound();
    }
    public void PlayButton2Sound()
    {
        Button2Sound();
    }
    public void PlaySelectionSound()
    {
        SelectionSound();
    }    
    public void PlayPurchaseSound()
    {
        PurchaseSound();
    }
    public void Button1Sound() => PlayClip(Button1Clip);
    public void Button2Sound() => PlayClip(Button2Clip);
    public void SelectionSound() => PlayClip(selectionClip);
    public void PurchaseSound() => PlayClip(PurchaseClip);
}
