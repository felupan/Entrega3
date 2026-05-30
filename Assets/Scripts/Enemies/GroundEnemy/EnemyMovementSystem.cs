using UnityEngine;

namespace Enemies.GroundEnemy
{
    public class EnemyMovementSystem : EnemySystem
    {
        [SerializeField] private GameObject target;

        private void Update()
        {
            main.Agent.SetDestination(target.transform.position);

            if (ReachedDestination())
            {
                main.Agent.isStopped = true;
                FaceToTarget();
            }
            else main.Agent.isStopped = false;
        }

        private void FaceToTarget()
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
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
