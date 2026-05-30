using System;
using DefaultNamespace.Enemies;
using DefaultNamespace.Interfaces;
using UnityEngine;

public class Parts : MonoBehaviour, IDamageable
{
    [SerializeField] private float multiplier = 1f;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    public void TakeDamage(float damage)
    {
        enemyHealth.TakeDamage(damage * multiplier);
    }
}
