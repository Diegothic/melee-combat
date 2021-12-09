using Camera;
using Camera.Controller;
using Character.Controller;
using Character.Movement;
using Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller
{
    public class PlayerController : MonoBehaviour, ICharacterController
    {
        private GameInputSchema _input;

        [SerializeField] private CharacterState state;
        [SerializeField] private float stateResetCdBase;
        private float _stateResetCd;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private CameraController _mainCameraController;

        [SerializeField] private Transform _target;
        private IMovement _movement;
        private Animator _animator;

        private void Awake()
        {
            state = CharacterState.Exploration;

            _rigidbody ??= GetComponent<Rigidbody>();
            _transform ??= GetComponent<Transform>();
            _mainCameraController ??= GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

            _input = new GameInputSchema();
            _movement ??= GetComponent<PlayerMovement>();
            _animator ??= GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _input.Player.SwitchState.performed += SwitchState;
            _input.Player.SwitchState.Enable();
        }

        private void OnDisable()
        {
            _input.Player.SwitchState.Disable();
        }

        public void EnableMovement()
        {
            _animator.applyRootMotion = false;
            _movement.Enable();
        }

        public void DisableMovement()
        {
            _movement.Disable();
            _animator.applyRootMotion = true;
        }

        public bool IsInCombat()
        {
            return state == CharacterState.Combat;
        }

        private void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (state != CharacterState.Combat || HasTarget())
                return;

            if (_stateResetCd > 0.0f)
            {
                _stateResetCd -= Time.deltaTime;
            }
            else
            {
                SetState(CharacterState.Exploration);
            }
        }

        public bool HasTarget()
        {
            return _target != null;
        }

        private void SwitchState(InputAction.CallbackContext ctx)
        {
            SetState(state == CharacterState.Exploration ? CharacterState.Combat : CharacterState.Exploration);
        }

        public void SetState(CharacterState newState)
        {
            if (newState == CharacterState.Combat)
                _stateResetCd = stateResetCdBase;
            else
                EnableMovement();
            state = newState;
        }

        public Vector3 TargetPosition()
        {
            return _target == null ? Vector3.zero : _target.position + Vector3.up * 1.3f;
        }

        public float AngleToTarget()
        {
            if (!HasTarget())
                return AngleToCamera();
            var vector = _target.position - transform.position;
            return Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg;
        }

        private float AngleToCamera()
        {
            var cameraForward = _mainCameraController.FlatForwardVector();
            return Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
        }

        public float AngleToVelocity()
        {
            var velocity = _rigidbody.velocity.normalized;
            return Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        }


        public float ForwardToMovementAngle()
        {
            var forward = _transform.forward;
            var velocity = _rigidbody.velocity.normalized;
            return velocity.magnitude < float.Epsilon
                ? 0.0f
                : Vector3.SignedAngle(forward, velocity, Vector3.up);
        }
    }
}