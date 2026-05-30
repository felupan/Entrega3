using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "InputReaderSO", menuName = "Scriptable Objects/InputReaderSO")]
    public class InputReaderSO : ScriptableObject, MyInputs.IGameplayActions
    {
        public event Action OnJumpStarted;
        public event Action OnShootStarted, OnShootCancel;
        public event Action OnAimStarted, OnAimCancel;
        public event Action OnDashStarted;
        public event Action<Vector2> OnMoveEvent;
        public event Action<int> OnWeaponSwitch;

        private MyInputs inputs;

        private void OnEnable()
        {
            inputs = new MyInputs();
            inputs.Gameplay.Enable();
            inputs.Gameplay.AddCallbacks(this);
        }

        private void OnDisable()
        {
            inputs.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started) OnJumpStarted?.Invoke();
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (context.started) OnShootStarted?.Invoke();
            else if (context.canceled) OnShootCancel?.Invoke();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.started) OnAimStarted?.Invoke();
            else if (context.canceled) OnAimCancel?.Invoke();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.started) OnDashStarted?.Invoke();
        }

        public void OnWeapon1(InputAction.CallbackContext context)
        {
            if (context.started) OnWeaponSwitch?.Invoke(0);
        }

        public void OnWeapon2(InputAction.CallbackContext context)
        {
            if (context.started) OnWeaponSwitch?.Invoke(1);
        }

        public void OnWeapon3(InputAction.CallbackContext context)
        {
            if (context.started) OnWeaponSwitch?.Invoke(2);
        }
    }
}
