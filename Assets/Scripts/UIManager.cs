using System;
using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image[] dashCharges;
    [SerializeField] private Image slowMotionFill;
    
    private float maxHealth;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void OnEnable()
    {
        PlayerHealthSystem.OnHealthChange += UpdateHealthBar;
        PlayerMovementSystem.OnDashUpdate += UpdateDashCharges;
        PlayerMovementSystem.OnSlowMotionChange += UpdateSlowMotion;
        GameManager.OnTimerUpdate += UpdateTimer;
    }


    private void OnDisable()
    {
        PlayerHealthSystem.OnHealthChange -= UpdateHealthBar;
        PlayerMovementSystem.OnDashUpdate -= UpdateDashCharges;
        PlayerMovementSystem.OnSlowMotionChange -= UpdateSlowMotion;
        GameManager.OnTimerUpdate -= UpdateTimer;
    }

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
    }
    
    private void UpdateHealthBar(float health)
    {
        healthBar.fillAmount = health / maxHealth;
        healthBar.transform.DOPunchPosition(Vector3.up, 0.5f);
    }

    private void UpdateTimer(float time)
    {
        timerText.SetText($"{Mathf.CeilToInt(time)}");
    }

    private void UpdateDashCharges(int charges, float rechargeProgress)
    {
        for (int i = 0; i < dashCharges.Length; i++)
        {
            if (i < charges)
            {
                dashCharges[i].fillAmount = 1;
            }
            else if (i == charges)
            {
                dashCharges[i].fillAmount = rechargeProgress;
            }
            else
            {
                dashCharges[i].fillAmount = 0;
            }
        }
    }
    private void UpdateSlowMotion(float amount)
    {
        slowMotionFill.fillAmount = amount;
    }
}
