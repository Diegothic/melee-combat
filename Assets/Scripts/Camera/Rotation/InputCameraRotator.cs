using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera.Rotation
{
    internal class InputCameraRotator : MonoBehaviour
    {
        public static event Action<string> OnInputDeviceChanged = delegate { };

        private GameInputSchema _input;
        private InputAction _pitch;
        private InputAction _yaw;

        [SerializeField]
        private float sensitivity;

        [SerializeField]
        private Vector2 pitchLimit;

        [SerializeField]
        private float rotationSmoothTime;
        private Vector3 _rotationSmoothVelocity;
        private Vector3 _currentRotation;

        private void Awake()
        {
            _input = new GameInputSchema();
        }

        private void OnEnable()
        {
            _pitch = _input.Camera.Pitch;
            _pitch.started += OnInput;
            _pitch.Enable();
            _yaw = _input.Camera.Yaw;
            _yaw.started += OnInput;
            _yaw.Enable();
        }

        private void OnDisable()
        {
            _pitch.Disable();
            _yaw.Disable();
        }

        private static void OnInput(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
        }

        public void SetCurrentRotation(Quaternion rotation)
        {
            _currentRotation = rotation.eulerAngles;
        }

        public Vector3 Rotation()
        {
            var yawDiff = _yaw.ReadValue<float>() * sensitivity * Time.deltaTime * 50.0f;
            var pitchDiff = _pitch.ReadValue<float>() * sensitivity * Time.deltaTime * 50.0f;

            var targetRotation = new Vector3(_currentRotation.x - pitchDiff, _currentRotation.y + yawDiff);
            targetRotation.x = Mathf.Clamp(targetRotation.x, pitchLimit.x, pitchLimit.y);
            _currentRotation =
                Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationSmoothVelocity, rotationSmoothTime);
            return _currentRotation;
        }
    }
}