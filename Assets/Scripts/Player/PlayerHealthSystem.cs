using System;
using System.Collections;
using DefaultNamespace.Interfaces;
using UnityEngine;

namespace Player
{
    public class PlayerHealthSystem : PlayerSystem, IDamageable
    {
        [SerializeField] private float baseHealth;
        public static event Action<float> OnHealthChange;
        public static event Action OnPlayerDeath; 
        private float currentHealth;

        private bool isAttackable;

        protected override void Awake()
        {
            base.Awake();
            currentHealth = baseHealth;
            isAttackable = true;
        }

        private void OnEnable()
        {
            OnHealthChange?.Invoke(currentHealth);
            UIManager.Instance.SetMaxHealth(baseHealth);
        }

        public void TakeDamage(float damage)
        {
            if (!isAttackable) return;
            isAttackable = false;
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnPlayerDeath?.Invoke();
            }
            OnHealthChange?.Invoke(currentHealth);
            StartCoroutine(TakeDamageCooldown());
        }

        private IEnumerator TakeDamageCooldown()
        {
            yield return new WaitForSeconds(0.5f);
            isAttackable = true;
        }
    }
}