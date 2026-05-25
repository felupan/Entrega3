using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Tracer : MonoBehaviour
    {
        private TrailRenderer trailRenderer;
        private Vector3 startPoint;
        private Vector3 endPoint;
        private float speed;
        private Action onComplete;

        private bool isComplete;

        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        public void Initialize(Vector3 start, Vector3 end, float speed, Action onComplete)
        {
            isComplete = false;
            endPoint = end;
            this.speed = speed;
            this.onComplete = onComplete;
            
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
                onComplete?.Invoke();
            }
        }
    }
}