using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerCameraSystem : PlayerSystem
    {
        [Header("Tilt Settings")]
        [SerializeField] private Transform cameraTilt;
        [SerializeField] private float tiltAmount;
        [SerializeField] private float tiltReturnTime;

        private Vector3 targetTilt;
        private Vector3 tiltVelocity;
        
        [Header("Fov Settings")]
        [SerializeField] private float baseFov;
        [SerializeField] private float dashFov;
        [SerializeField] private float fovReturnTime;
        
        private float currentFov;

        private void Start()
        {
            main.CineCam.Lens.FieldOfView = baseFov;
            currentFov = baseFov;
        }

        private void Update()
        {
            main.CineCam.Lens.FieldOfView = currentFov;
            cameraTilt.localEulerAngles = targetTilt;
            
            currentFov = Mathf.Lerp(currentFov, baseFov, fovReturnTime * Time.deltaTime);

            if (targetTilt.sqrMagnitude > 0)
            {
                targetTilt = Vector3.Lerp(targetTilt, Vector3.zero, tiltReturnTime * Time.deltaTime);
            }
        }

        public void ApplyDashFovEffect(Vector3 direction)
        {
            targetTilt = new Vector3(direction.z * tiltAmount, 0, 0);
            currentFov = dashFov;
        }
    }
}