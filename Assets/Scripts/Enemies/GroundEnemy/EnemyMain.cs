using System;
using DefaultNamespace.Enemies;
using Enemies.GroundEnemy;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMain : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    public EnemyHealth Health { get; private set; }
    public EnemyMovementSystem MovementSystem { get; private set; }
    public Animator Anim { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<EnemyHealth>();
        MovementSystem = GetComponent<EnemyMovementSystem>();
        Anim = GetComponentInChildren<Animator>();
    }
}
