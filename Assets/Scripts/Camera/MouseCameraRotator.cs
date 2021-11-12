using UnityEngine;

internal class MouseCameraRotator : MonoBehaviour, ICameraRotator
{
    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Vector2 pitchLimit;

    [SerializeField] private float rotationSmoothTime;
    private Vector3 _rotationSmoothVelocity;
    private Vector3 _currentRotation;
    private float _yaw;
    private float _pitch;

    public Vector3 RotateCamera()
    {
        _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, pitchLimit.x, pitchLimit.y);

        var targetRotation = new Vector3(_pitch, _yaw);
        _currentRotation =
            Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationSmoothVelocity, rotationSmoothTime);
        return _currentRotation;
    }
}