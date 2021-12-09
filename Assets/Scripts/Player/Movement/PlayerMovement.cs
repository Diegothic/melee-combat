using Camera.Controller;
using Character.Controller;
using Character.Movement;
using Input;
using Player.Controller;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour, IMovement, IMovementInputReceiver
    {
        private bool _enabled;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float inCombatMovementSpeed;
        [SerializeField] private float walkSpeedPercent;
        private bool _shouldWalk;
        private bool _receivesInput;

        private float _targetRotation;
        [SerializeField] private float turnSmoothTime;
        private float _turnSmoothVelocity;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private IPlayerInput _playerInput;
        private ICharacterController _characterController;
        private CameraController _mainCameraController;

        private void Awake()
        {
            _enabled = true;
            _targetRotation = transform.eulerAngles.y;

            _transform ??= GetComponent<Transform>();
            _rigidbody ??= GetComponent<Rigidbody>();
            _playerInput ??= GetComponent<IPlayerInput>();
            _characterController ??= GetComponent<PlayerController>();
            _mainCameraController ??= GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        }

        private void Update()
        {
            if (!_enabled)
                return;
            Rotate();
            Move();
        }

        public void Enable()
        {
            _enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }

        public void EnableWalk(InputAction.CallbackContext ctx)
        {
            _shouldWalk = true;
        }

        public void DisableWalk(InputAction.CallbackContext ctx)
        {
            _shouldWalk = false;
        }

        public bool IsMoving()
        {
            return _receivesInput;
        }

        private void Move()
        {
            _receivesInput = true;
            var input = _playerInput.InputAxis();

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
            if (_characterController.IsInCombat())
                return inCombatMovementSpeed;

            return _shouldWalk ? movementSpeed * walkSpeedPercent : movementSpeed;
        }

        private void Rotate()
        {
            _transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation,
                ref _turnSmoothVelocity, turnSmoothTime);
            if (!IsMoving())
                return;

            _targetRotation = _characterController.IsInCombat()
                ? _characterController.AngleToTarget()
                : _characterController.AngleToVelocity();
        }

        public float SpeedPercent()
        {
            return IsMoving() ? _rigidbody.velocity.magnitude / movementSpeed : 0.0f;
        }
    }
}