using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerCameraSystem : PlayerSystem
    {
        [Header("Tilt Settings")]
        [SerializeField] private Transform cameraTilt;
        [SerializeField] private float tiltAmount;
        [SerializeField] private float tiltReturnTime;
        
        [Header("Fov Settings")]
        [SerializeField] private float baseFov;
        [SerializeField] private float fovReturnTime;
        
        [Header("Camera Settings")]
        [SerializeField] private float cameraSens = 1.2f;
        [SerializeField] private float baseAccelTime = 0.2f;
        [SerializeField] private float baseDecelTime = 0.2f;

        [Header("Global Volume Settings")] 
        [SerializeField] private Volume globalVolume;
        [SerializeField] private float vignetteIntensity;
        [SerializeField] private float aberrationIntensity;

        private Vignette vignette;
        private ChromaticAberration chromaticAberration;
        
        private float currentVignetteIntensity;
        private float currentAberrationIntensity;
        
        private Vector3 targetTilt;
        private Vector3 tiltVelocity;

        private CinemachineInputAxisController inputAxisController;
        
        private float currentFov;
        private Vector3 currentTilt;

        protected override void Awake()
        {
            base.Awake();
            inputAxisController = main.CineCam.GetComponent<CinemachineInputAxisController>();
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out chromaticAberration);
        }

        private void Start()
        {
            main.CineCam.Lens.FieldOfView = baseFov;
            currentFov = baseFov;
        }

        private void Update()
        {
            main.CineCam.Lens.FieldOfView = currentFov;
            
            currentFov = Mathf.Lerp(currentFov, baseFov, fovReturnTime * Time.deltaTime);

            currentTilt = Vector3.Lerp(currentTilt, targetTilt, tiltReturnTime * Time.deltaTime);
            cameraTilt.localEulerAngles = currentTilt;
            
            UpdateSens();
            SlowMotionEffect();
        }

        public void ApplyDashFovEffect(Vector3 direction)
        {
            currentFov = baseFov - 5;
        }
        
        public void UpdateTilt(Vector2 input)
        {
            float tilt = Mathf.Abs(input.x) > Mathf.Abs(input.y) ? -input.x * tiltAmount : 0;
            targetTilt = new Vector3(0, 0, tilt);
        }

        private void UpdateSens()
        {
            float sensitivityMultiplier = Mathf.Lerp(cameraSens, cameraSens / Time.timeScale, 0.7f);

            foreach (var controller in inputAxisController.Controllers)
            {
                controller.Driver.AccelTime = baseAccelTime * (cameraSens / sensitivityMultiplier);
                controller.Driver.DecelTime = baseDecelTime * (cameraSens / sensitivityMultiplier);
            }
            
            inputAxisController.Controllers[0].Input.Gain = cameraSens * sensitivityMultiplier;
            inputAxisController.Controllers[1].Input.Gain = -cameraSens * sensitivityMultiplier;
        }

        private void SlowMotionEffect()
        {
            float targetAberrationInt;
            float targetVignetteInt;
            
            if (main.MovementSystem.IsSlowMotion)
            {
                targetAberrationInt = aberrationIntensity;
                targetVignetteInt = vignetteIntensity;
            }
            else
            {
                targetAberrationInt = 0;
                targetVignetteInt = 0;
            }
            currentAberrationIntensity = Mathf.Lerp(currentAberrationIntensity, targetAberrationInt,10 * Time.unscaledDeltaTime);
            currentVignetteIntensity = Mathf.Lerp(currentVignetteIntensity, targetVignetteInt, 10 * Time.unscaledDeltaTime);
            
            chromaticAberration.intensity.value = currentAberrationIntensity;
            vignette.intensity.value = currentVignetteIntensity;
        }
    }
}