using System;
using DefaultNamespace.Enemies;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [field:SerializeField] public float timeRemaining { get; private set; }
    [SerializeField] private AudioClip gameMusic;
    private bool gameOver;
    private int lastSecond;

    public static event Action<float> OnTimerUpdate;  
    public static GameManager Instance { get; private set; }

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

    private void Start()
    {
        AudioManager.Instance.PlayMusic(gameMusic);
    }

    private void OnEnable()
    {
        PlayerHealthSystem.OnPlayerDeath += ChangeToDeathScreen;
    }

    private void OnDisable()
    {
        PlayerHealthSystem.OnPlayerDeath -= ChangeToDeathScreen;
    }

    private void Update()
    {
        if (gameOver) return;
        timeRemaining -= Time.deltaTime;
        int currentSecond = Mathf.CeilToInt(timeRemaining);
        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            OnTimerUpdate?.Invoke(timeRemaining);
        }
        
        if (timeRemaining <= 0)
        {
            SceneManager.LoadScene("Victory");
        }
    }

    private void ChangeToDeathScreen()
    {
        SceneManager.LoadScene("Death");
    }
    
    
}
