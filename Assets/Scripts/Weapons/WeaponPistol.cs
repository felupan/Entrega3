using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Interfaces;
using UnityEngine;

namespace Weapons
{
    public class WeaponPistol : WeaponRange
    {
        [SerializeField] private AudioClip shotSfx;
        private Vector3 endPoint;
        
        protected override void OnShootCanceled(){}

        protected override void OnShoot()
        {
            Ray ray = main.Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            
            if (Physics.Raycast(ray, out RaycastHit hit, range, ~LayerMask.GetMask("Player")))
            {
                endPoint = hit.point;
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                }
                //Debug.Log($"Hit: {hit.transform.name}");
            }
            else
            {
                endPoint = ray.origin + ray.direction * range;
            }
            
            SpawnTracer(muzzlePoint.position, endPoint, bulletSpeed);
            
            AudioManager.Instance.PlaySfx(shotSfx);
        }
    }
}