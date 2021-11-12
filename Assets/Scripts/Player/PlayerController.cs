using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody _rigidbody;
    private CameraController _mainCameraController;

    private bool _isMoving = false;

    private float _targetRotation;
    [SerializeField] private float turnSmoothTime;
    private float _turnSmoothVelocity;

    [SerializeField] private bool rotateToCamera;

    private void Awake()
    {
        _rigidbody ??= GetComponent<Rigidbody>();
        _mainCameraController ??= GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        _targetRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        var input = GetInputAxis();

        _isMoving = true;

        if (input.magnitude < 0.01f)
        {
            _isMoving = false;
        }

        var desiredVelocity = new Vector3(0, _rigidbody.velocity.y, 0);
        desiredVelocity += input.x * movementSpeed * _mainCameraController.GetLookRight();
        desiredVelocity += input.y * movementSpeed * _mainCameraController.GetLookForward();
        _rigidbody.velocity = desiredVelocity;
    }

    private static Vector2 GetInputAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Rotate()
    {
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
            ref _turnSmoothVelocity, turnSmoothTime);
        if (!_isMoving)
        {
            return;
        }

        if (rotateToCamera)
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
        var cameraForward = _mainCameraController.GetLookForward();
        _targetRotation = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
    }

    private void RotationToMovement()
    {
        var velocity = _rigidbody.velocity.normalized;
        _targetRotation = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
    }
}