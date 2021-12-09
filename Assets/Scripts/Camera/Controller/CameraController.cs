using Camera.Rotation;
using Camera.Transition;
using Player.Controller;
using UnityEngine;

namespace Camera.Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 combatOffset;
        [SerializeField] private float followSpeed;

        private Transform _transform;
        private Transform _followTarget;
        private PlayerController _playerController;
        private ICameraRotator _cameraRotator;
        private CameraTransitionController _transitionController;

        private void Awake()
        {
            _transform = GetComponent<Transform>();

            var playerObject = GameObject.FindGameObjectWithTag("Player");
            _followTarget ??= playerObject.transform;
            _playerController ??= playerObject.GetComponent<PlayerController>();

            _cameraRotator ??= GetComponent<ICameraRotator>();
            _transitionController ??= GetComponent<CameraTransitionController>();
        }

        public Vector3 FlatForwardVector()
        {
            return FlattenVector(_transform.forward);
        }

        public Vector3 FlatRightVector()
        {
            return FlattenVector(_transform.right);
        }

        private static Vector3 FlattenVector(Vector3 vector)
        {
            var result = vector;
            result.y = 0;
            return result.normalized;
        }

        private void Update()
        {
            UpdateTransitionController();
        }

        private void UpdateTransitionController()
        {
            var desiredPosition = CalculateDesiredPosition();
            _transitionController.UpdateDesiredPosition(desiredPosition);
        }

        private void FixedUpdate()
        {
            _transitionController.CheckForTransition();
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (_transitionController.IsTransitioning())
                return;

            UpdateRotation();
            FollowTarget();
        }

        private void UpdateRotation()
        {
            if (_playerController.HasTarget())
            {
                transform.LookAt(_playerController.TargetPosition());
                return;
            }

            transform.eulerAngles = _cameraRotator.Rotation();
        }

        private void FollowTarget()
        {
            transform.position = Vector3.Lerp(transform.position, CalculateDesiredPosition(), followSpeed);
        }

        private Vector3 CalculateDesiredPosition()
        {
            var selectedOffset = _playerController.IsInCombat() ? combatOffset : offset;

            var desiredPosition = _followTarget.position - _transform.forward * selectedOffset.z +
                                  _transform.right * selectedOffset.x;
            desiredPosition.y += selectedOffset.y;
            return desiredPosition;
        }
    }
}