using System;
using DefaultNamespace.Enemies;
using DefaultNamespace.Interfaces;
using UnityEngine;

public class Parts : MonoBehaviour, IDamageable
{
    [SerializeField] private float multiplier = 1f;
    private EnemyHealth enemyHealth;
    private EnemyAttack enemyAttack;

    private void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        enemyAttack = GetComponentInParent<EnemyAttack>();
    }

    private void OnCollisionEnter(Collision other)
    {
        enemyAttack.Attack(other);
    }

    public void TakeDamage(float damage)
    {
        enemyHealth.TakeDamage(damage * multiplier);
    }
}
