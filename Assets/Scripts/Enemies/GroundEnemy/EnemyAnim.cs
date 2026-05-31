using System;
using UnityEngine;

namespace Enemies.GroundEnemy
{
    public class EnemyAnim : EnemySystem
    {
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Update()
        {
            float currentSpeed = main.Agent.velocity.magnitude / main.Agent.speed;
            main.Anim.SetFloat(Speed, currentSpeed);
        }
    }
}