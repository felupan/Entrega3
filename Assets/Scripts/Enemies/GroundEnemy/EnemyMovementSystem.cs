using System;
using UnityEngine;

namespace Enemies.GroundEnemy
{
    public class EnemyMovementSystem : EnemySystem
    {
        private Transform target;

        private void Start()
        {
            main.Agent.stoppingDistance = 1;
        }

        private void OnEnable()
        {
            main.Health.OnDeath += StopMoving;
        }

        private void StopMoving()
        {
            main.Agent.speed = 0;
        }

        private void Update()
        {
            main.Agent.SetDestination(target.position);

            if (ReachedDestination())
            {
                main.Agent.isStopped = true;
                FaceToTarget();
            }
            else main.Agent.isStopped = false;
        }
        
        public void SetTarget(Transform t)
        {
            target = t;
        }

        private void FaceToTarget()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0f;
        
            Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
            transform.rotation = rotationToTarget;
        }

        private bool ReachedDestination()
        {
            return !main.Agent.pathPending && main.Agent.remainingDistance <= main.Agent.stoppingDistance;
        }
    }
}
