using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Tracer : MonoBehaviour
    {
        [SerializeField] private float lingerTime;
        private TrailRenderer trailRenderer;
        private Vector3 startPoint;
        private Vector3 endPoint;
        private float speed;
        private Action onComplete;

        private bool isComplete;
        private float initialWidth;

        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            initialWidth = trailRenderer.startWidth;
        }

        public void Initialize(Vector3 start, Vector3 end, float speed, Action onComplete)
        {
            isComplete = false;
            endPoint = end;
            this.speed = speed;
            this.onComplete = onComplete;
            trailRenderer.startWidth = initialWidth;
            trailRenderer.endWidth = initialWidth;
            
            trailRenderer.enabled = false;
            transform.position = start;
            trailRenderer.enabled = true;
            trailRenderer.Clear();
        }

        private void Update()
        {
            if (isComplete) return;

            transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);

            if (transform.position == endPoint)
            {
                isComplete = true;
                StartCoroutine(DelayedComplete());
            }
        }
        
        private IEnumerator DelayedComplete()
        {
            float elapsed = 0;
    
            while (elapsed < lingerTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lingerTime;
                trailRenderer.startWidth = Mathf.Lerp(initialWidth, 0, t);
                trailRenderer.endWidth = Mathf.Lerp(initialWidth, 0, t);
                yield return null;
            }
    
            onComplete?.Invoke();
        }
    }
}