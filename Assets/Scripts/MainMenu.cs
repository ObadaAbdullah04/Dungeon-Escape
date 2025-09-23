using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGameButton()
    {
        MM_AudioManager.Instance.PlayStartSound();
        StartCoroutine(DelayThenLoad(MM_AudioManager.Instance.startClip));
    }
    public void QuitButton()
    {
        MM_AudioManager.Instance.PlayQuitSound();
        StartCoroutine(DelayQuit(MM_AudioManager.Instance.QuitClip));
    }

    IEnumerator DelayThenLoad(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
        SceneManager.LoadScene("Game");
    }
    IEnumerator DelayQuit(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
        Application.Quit();
    }
}
