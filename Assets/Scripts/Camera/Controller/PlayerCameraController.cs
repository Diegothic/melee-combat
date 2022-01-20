using System.Collections;
using Camera.Rotation;
using Camera.Transition;
using Character;
using Character.Combat;
using UnityEngine;

namespace Camera.Controller
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private Vector3 combatOffset;
        [SerializeField]
        private float followSpeed;
        [SerializeField]
        private float targetFollowSpeed;

        [SerializeField]
        private LayerMask collisionMask;

        private Transform _transform;
        private Transform _followTarget;
        private Targeting _targeting;
        private InputCameraRotator _cameraRotator;
        private CameraTransitionController _transitionController;

        private void Awake()
        {
            _transform = GetComponent<Transform>();

            var playerObject = GameObject.FindGameObjectWithTag("Player");
            _followTarget ??= playerObject.transform;
            _targeting ??= playerObject.GetComponent<Targeting>();

            _cameraRotator ??= GetComponent<InputCameraRotator>();
            _transitionController ??= GetComponent<CameraTransitionController>();

            var combats = FindObjectsOfType<HumanoidCombat>();
            foreach (var combat in combats)
            {
                combat.OnAttack += Shake;
            }
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
            PreventClipping();
        }

        private void UpdateRotation()
        {
            var speed = targetFollowSpeed * Time.deltaTime;
            Quaternion targetRotation;
            if (_targeting.HasTarget())
            {
                var lookVector = _targeting.Target.position + Vector3.up * 1.2f + _followTarget.forward -
                                 _transform.position;
                targetRotation = Quaternion.LookRotation(lookVector);
                _cameraRotator.SetCurrentRotation(targetRotation);
            }
            else
            {
                speed *= 10.0f;
                targetRotation = Quaternion.Euler(_cameraRotator.Rotation());
            }

            transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, speed);
        }

        private void FollowTarget()
        {
            transform.position = Vector3.Lerp(transform.position, CalculateDesiredPosition(), followSpeed);
        }

        private Vector3 CalculateDesiredPosition()
        {
            var selectedOffset = _targeting.HasTarget() ? combatOffset : offset;

            var desiredPosition = _followTarget.position - _transform.forward * selectedOffset.z +
                                  _transform.right * selectedOffset.x;
            desiredPosition.y += selectedOffset.y;
            return desiredPosition;
        }

        private void PreventClipping()
        {
            var selectedOffset = _targeting.HasTarget() ? combatOffset : offset;

            var followPosition = _followTarget.position + Vector3.up * 1.2f;
            var castVec = (_transform.position - followPosition).normalized;
            var didHit = Physics.Raycast(followPosition, castVec, out var hit, selectedOffset.z - 0.05f,
                collisionMask);
            if (didHit)
            {
                _transform.position = hit.point + castVec * -0.05f;
            }
        }

        private void Shake()
        {
            StopCoroutine("CShake");
            StartCoroutine(CShake(0.1f, 0.1f));
        }

        private IEnumerator CShake(float time, float strength)
        {
            var elapsed = 0.0f;
            while (elapsed < time)
            {
                var shakeVec = new Vector3(Random.Range(-strength, strength),
                    Random.Range(-strength, strength),
                    Random.Range(-strength, strength));
                var shake = Vector3.Lerp(_transform.position, _transform.position + shakeVec, Time.deltaTime * 50.0f);
                _transform.position = shake;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}