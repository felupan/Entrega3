using System;
using Player;
using UnityEngine;

public class WeaponAnimationSystem : PlayerSystem
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsShooting = Animator.StringToHash("isShooting");
    private Animator anim;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        main.InputReader.OnShootStarted += ShootAnim;
    }

    private void OnDisable()
    {
        main.InputReader.OnShootStarted -= ShootAnim;
    }

    private void ShootAnim()
    {
        anim.SetTrigger(IsShooting);
    }

    private void Update()
    {
        anim.SetBool(IsRunning, main.MovementSystem.IsMoving);
    }
}
