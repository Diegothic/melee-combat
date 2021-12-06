using UnityEngine;
using UnityEngine.InputSystem;

internal class InputCameraRotator : MonoBehaviour, ICameraRotator
{
    private PlayerInput _input;
    private InputAction _pitch;
    private InputAction _yaw;

    [SerializeField] private float sensitivity;

    [SerializeField] private Vector2 pitchLimit;

    [SerializeField] private float rotationSmoothTime;
    private Vector3 _rotationSmoothVelocity;
    private Vector3 _currentRotation;
    private float _currentPitch;
    private float _currentYaw;

    private void Awake()
    {
        _input = new PlayerInput();
    }

    private void OnEnable()
    {
        _pitch = _input.Camera.Pitch;
        _pitch.Enable();
        _yaw = _input.Camera.Yaw;
        _yaw.Enable();
    }

    private void OnDisable()
    {
        _pitch.Disable();
        _yaw.Disable();
    }

    public Vector3 RotateCamera()
    {
        _currentYaw += _yaw.ReadValue<float>() * sensitivity;
        _currentPitch -= _pitch.ReadValue<float>() * sensitivity;
        _currentPitch = Mathf.Clamp(_currentPitch, pitchLimit.x, pitchLimit.y);

        var targetRotation = new Vector3(_currentPitch, _currentYaw);
        _currentRotation =
            Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationSmoothVelocity, rotationSmoothTime);
        return _currentRotation;
    }
}