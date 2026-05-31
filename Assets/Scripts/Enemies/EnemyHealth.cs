using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Enemies
{
    public class EnemyHealth : MonoBehaviour
    {
        public event Action OnDeath;
        [SerializeField] private float baseHealth;
        private float currentHealth;
        private void Awake()
        {
            currentHealth = baseHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0) Die();
        }

        private void Die()
        {
            OnDeath?.Invoke();
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0;
            float duration = 0.5f;
            Vector3 initialScale = transform.localScale;
    
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / duration);
                yield return null;
            }
    
            Destroy(gameObject);
        }
    }
}