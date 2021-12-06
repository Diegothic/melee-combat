using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovement
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float inCombatMovementSpeed;
    [SerializeField] private float walkSpeedPercent;
    private bool _receivesInput;

    private float _targetRotation;
    [SerializeField] private float turnSmoothTime;
    private float _turnSmoothVelocity;

    private Transform _transform;
    private Rigidbody _rigidbody;
    private PlayerController _playerController;
    private CameraController _mainCameraController;

    private void Awake()
    {
        _transform ??= GetComponent<Transform>();
        _rigidbody ??= GetComponent<Rigidbody>();
        _playerController ??= GetComponent<PlayerController>();
        _mainCameraController ??= GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        _targetRotation = transform.eulerAngles.y;
    }

    public void Move()
    {
        _receivesInput = true;
        var input = _playerController.InputAxis();

        var desiredVelocity = new Vector3(0, _rigidbody.velocity.y, 0);
        if (input.magnitude < float.Epsilon)
        {
            _receivesInput = false;
        }
        else
        {
            desiredVelocity += input.x * MovementSpeed() * _mainCameraController.FlatRightVector();
            desiredVelocity += input.y * MovementSpeed() * _mainCameraController.FlatForwardVector();
        }

        _rigidbody.velocity = desiredVelocity;
    }

    private float MovementSpeed()
    {
        if (_playerController.IsInCombat())
            return inCombatMovementSpeed;

        return _playerController.ShouldWalk() ? movementSpeed * walkSpeedPercent : movementSpeed;
    }

    public void Rotate()
    {
        _transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation,
            ref _turnSmoothVelocity, turnSmoothTime);
        if (!IsMoving())
            return;

        _targetRotation = _playerController.IsInCombat() ? RotationToCamera() : RotationToMovement();
    }

    private bool IsMoving()
    {
        return _receivesInput;
    }

    private float RotationToCamera()
    {
        var cameraForward = _mainCameraController.FlatForwardVector();
        return Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
    }

    private float RotationToMovement()
    {
        var velocity = _rigidbody.velocity.normalized;
        return Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
    }

    public float SpeedPercent()
    {
        return IsMoving() ? _rigidbody.velocity.magnitude / movementSpeed : 0.0f;
    }

    public float ForwardToMovementAngle()
    {
        if (!IsMoving())
            return 0.0f;

        var forward = _transform.forward;
        var velocity = _rigidbody.velocity.normalized;

        return Vector3.SignedAngle(forward, velocity, Vector3.up);
    }
}