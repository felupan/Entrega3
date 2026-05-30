using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMain : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    [SerializeField] private float maxHealth;
    public float CurrentHealth { get; protected set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }
}
