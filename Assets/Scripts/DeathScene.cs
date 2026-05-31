using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MainMenu());
    }

    private IEnumerator MainMenu()
    {
        yield return new WaitForSeconds(4f);
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene("MainMenu");
    }
}
