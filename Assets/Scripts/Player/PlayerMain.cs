using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public class PlayerMain : MonoBehaviour
    {
        public CharacterController Controller { get; private set; }
        public Camera Cam { get; private set; }
        public CinemachineCamera CineCam { get; private set; }
        public PlayerCameraSystem CameraSystem { get; private set; }
        public PlayerMovementSystem MovementSystem { get; private set; }

        private void Awake()
        {
            Controller = GetComponentInChildren<CharacterController>();
            Cam = GetComponentInChildren<Camera>();
            CameraSystem = GetComponentInChildren<PlayerCameraSystem>();
            CineCam = GetComponentInChildren<CinemachineCamera>();
            MovementSystem = GetComponentInChildren<PlayerMovementSystem>();
        }
    }
}
