using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour, IMovement
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float combatMovementSpeed;
    private const float WalkSpeedPercent = 0.3f;
    private bool _isMoving;

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

    private bool IsMoving()
    {
        return _isMoving;
    }

    public void Move()
    {
        _isMoving = true;
        var input = PlayerController.InputAxis();

        if (input.magnitude < 0.01f)
        {
            _isMoving = false;
        }

        var desiredVelocity = new Vector3(0, _rigidbody.velocity.y, 0);
        desiredVelocity += input.x * MovementSpeed() * _mainCameraController.FlatRightVector();
        desiredVelocity += input.y * MovementSpeed() * _mainCameraController.FlatForwardVector();
        _rigidbody.velocity = desiredVelocity;
    }

    private float MovementSpeed()
    {
        if (_playerController.IsInCombat())
        {
            return combatMovementSpeed;
        }

        if (ShouldWalk())
        {
            return movementSpeed * WalkSpeedPercent;
        }

        return movementSpeed;
    }

    private static bool ShouldWalk()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }

    public void Rotate()
    {
        _transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation,
            ref _turnSmoothVelocity, turnSmoothTime);
        if (!IsMoving())
        {
            return;
        }

        if (_playerController.IsInCombat())
        {
            RotationToCamera();
        }
        else
        {
            RotationToMovement();
        }
    }

    private void RotationToCamera()
    {
        var cameraForward = _mainCameraController.FlatForwardVector();
        _targetRotation = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
    }

    private void RotationToMovement()
    {
        var velocity = _rigidbody.velocity.normalized;
        _targetRotation = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
    }

    public float SpeedPercent()
    {
        if (!IsMoving())
        {
            return 0.0f;
        }

        return _rigidbody.velocity.magnitude / movementSpeed;
    }

    public float ForwardToMovementAngle()
    {
        if (!IsMoving())
        {
            return 0.0f;
        }

        var forward = _transform.forward;
        var velocity = _rigidbody.velocity.normalized;

        return Vector3.SignedAngle(forward, velocity, Vector3.up);
    }
}