using System;
using Player;
using ScriptableObjects;
using UnityEngine;

public abstract class WeaponBase : PlayerSystem
{
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] protected Transform muzzlePoint;
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float bulletSpeed;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        inputReader.OnShootStarted += OnShoot;
        inputReader.OnShootCancel += OnShootCanceled;
    }

    private void OnDisable()
    {
        inputReader.OnShootStarted -= OnShoot;
        inputReader.OnShootCancel -= OnShootCanceled;
    }

    protected abstract void OnShootCanceled();
    protected abstract void OnShoot();
}
