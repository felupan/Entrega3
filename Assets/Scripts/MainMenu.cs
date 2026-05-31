using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private Image tutorial;
    private void Start()
    {
        AudioManager.Instance.PlayMusic(menuMusic);
    }

    public void PlayButton()
    {
        StartCoroutine(ShowTutorial());
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    private IEnumerator ShowTutorial()
    {
        tutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(8f);
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene("MainMap");
    }
}
