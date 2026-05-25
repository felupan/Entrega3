using DefaultNamespace.Interfaces;
using UnityEngine;

namespace Weapons
{
    public class WeaponPistol : WeaponRange
    {
        private Vector3 endPoint;
        protected override void OnShootCanceled(){}

        protected override void OnShoot()
        {
            Ray ray = main.Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                endPoint = hit.point;
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                }
            }
            else
            {
                endPoint = ray.origin + ray.direction * range;
            }
            
            SpawnTracer(muzzlePoint.position, endPoint, bulletSpeed);
        }
    }
}