using Character.Animations;
using Character.LookDirection;
using Character.State;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        public bool MovementEnabled { get; set; }
        public bool RotationEnabled { get; set; }

        [SerializeField]
        private bool usesCamera;

        [SerializeField]
        private float movementSpeed;
        [SerializeField]
        private float inCombatMovementSpeed;
        [SerializeField]
        private float walkSpeedPercent;
        private bool _isMoving;

        private Vector2 _direction;

        private float _targetRotation;
        [SerializeField]
        private float turnSmoothTime;
        private float _turnSmoothVelocity;

        private ILookDirection _lookDirection;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private MovementAnimator _animator;

        private void Awake()
        {
            MovementEnabled = true;
            RotationEnabled = true;

            _targetRotation = transform.eulerAngles.y;

            _transform ??= GetComponent<Transform>();
            _rigidbody ??= GetComponent<Rigidbody>();
            _animator ??= GetComponent<MovementAnimator>();

            _lookDirection = usesCamera
                ? new CameraLookDirection() as ILookDirection
                : new TransformLookDirection(_transform);
        }

        private void Update()
        {
            _animator.SetSpeedPercent(SpeedPercent());
            _animator.SetForwardToMovementAngle(ForwardToMovementAngle());
        }

        public void Move(Vector2 direction, CharacterState state)
        {
            _direction = direction;
            if (!MovementEnabled)
            {
                _isMoving = false;
                return;
            }

            _isMoving = true;

            var desiredVelocity = new Vector3(0, _rigidbody.velocity.y, 0);
            if (direction.magnitude < float.Epsilon)
            {
                _isMoving = false;
            }
            else
            {
                desiredVelocity += direction.x * MovementSpeed(state) * _lookDirection.Right();
                desiredVelocity += direction.y * MovementSpeed(state) * _lookDirection.Forward();
            }

            _rigidbody.velocity = desiredVelocity;
        }

        private float SpeedPercent()
        {
            return _isMoving ? _rigidbody.velocity.magnitude / movementSpeed : 0.0f;
        }

        private float ForwardToMovementAngle()
        {
            var forward = _transform.forward;
            var velocity = _rigidbody.velocity.normalized;
            return velocity.magnitude < float.Epsilon
                ? 0.0f
                : Vector3.SignedAngle(forward, velocity, Vector3.up);
        }

        private float MovementSpeed(CharacterState state)
        {
            return state switch
            {
                CharacterState.Combat => inCombatMovementSpeed,
                CharacterState.Exploration => movementSpeed,
                CharacterState.Walk => movementSpeed * walkSpeedPercent,
                _ => movementSpeed
            };
        }

        public void Rotate(Transform target)
        {
            if (!RotationEnabled) return;

            _transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation,
                ref _turnSmoothVelocity, turnSmoothTime);
            if (!_isMoving && target == null) return;

            _targetRotation = target == null ? AngleToVelocity() : AngleToTarget(target);
        }

        public void InstantRotate(Transform target, bool useLookDirection, float speed)
        {
            var angle = target == null ? AngleToDirection(useLookDirection) : AngleToTarget(target);

            _targetRotation = angle;
            _transform.eulerAngles =
                Vector3.up * Mathf.LerpAngle(_transform.eulerAngles.y, angle, speed * 10.0f * Time.deltaTime);
        }

        private float AngleToTarget(Transform target)
        {
            var vector = target.position - transform.position;
            return Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg;
        }

        private float AngleToVelocity()
        {
            var velocity = _rigidbody.velocity.normalized;
            return Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        }

        private float AngleToDirection(bool useLookDirection)
        {
            return useLookDirection
                ? AngleToLookDirection()
                : AngleToMovementDirection();
        }

        private float AngleToMovementDirection()
        {
            var direction = new Vector3();
            direction += _direction.x * _lookDirection.Right();
            direction += _direction.y * _lookDirection.Forward();
            if (direction.magnitude < float.Epsilon)
                return _transform.eulerAngles.y;
            direction = direction.normalized;
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        private float AngleToLookDirection()
        {
            var direction = _lookDirection.Forward();
            direction = direction.normalized;
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * SpeedPercent());
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up, transform.right);
        }
    }
}