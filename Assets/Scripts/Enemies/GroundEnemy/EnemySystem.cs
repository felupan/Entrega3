using System;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    protected EnemyMain main;

    private void Awake()
    {
        main = transform.root.GetComponent<EnemyMain>();
    }
}
