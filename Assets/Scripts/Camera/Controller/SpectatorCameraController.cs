using System.Collections;
using Character.Combat;
using UnityEngine;

namespace Camera.Controller
{
    public class SpectatorCameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform target1;
        [SerializeField]
        private Transform target2;

        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private float followSpeed;
        [SerializeField]
        private float distanceApartMultiplier;

        [SerializeField]
        private float lookHeight;

        [SerializeField]
        private LayerMask collisionMask;

        private Transform _cameraTransform;

        private void Awake()
        {
            _cameraTransform ??= GetComponent<Transform>();

            var combats = FindObjectsOfType<HumanoidCombat>();
            foreach (var combat in combats)
            {
                combat.OnAttack += Shake;
            }
        }

        private void FixedUpdate()
        {
            UpdatePosition();
            UpdateRotation();
            PreventClipping();
        }

        private void UpdatePosition()
        {
            var betweenVec = target2.position - target1.position;
            var distanceApart = betweenVec.magnitude;
            var center = target1.position + betweenVec * 0.5f;
            var perpendicular = Vector2.Perpendicular(new Vector2(betweenVec.x, betweenVec.z));
            var perpendicular3 = new Vector3(perpendicular.x, 0.0f, perpendicular.y).normalized;

            var desiredPos = center + perpendicular3
                                    + perpendicular3 * offset.z
                                    + Vector3.up * offset.y
                                    + betweenVec.normalized * offset.x;
            desiredPos += perpendicular3 * distanceApart * distanceApartMultiplier;
            var newPos = Vector3.Lerp(_cameraTransform.position, desiredPos, followSpeed * Time.deltaTime);
            _cameraTransform.position = newPos;
        }

        private void UpdateRotation()
        {
            var betweenVec = target2.position - target1.position;
            var center = target1.position + betweenVec * 0.5f + Vector3.up * lookHeight;
            var desiredRot = Quaternion.LookRotation(center - _cameraTransform.position, Vector3.up);
            var newRot = Quaternion.Slerp(transform.rotation, desiredRot, followSpeed * Time.deltaTime);
            transform.rotation = newRot;
        }

        private void PreventClipping()
        {
            var betweenVec = target2.position - target1.position;
            var center = target1.position + betweenVec * 0.5f;
            var castVec = (_cameraTransform.position - center).normalized;
            var didHit = Physics.Raycast(center, castVec, out var hit, offset.z + 1.05f,
                collisionMask);
            if (didHit)
            {
                _cameraTransform.position = hit.point + castVec * -0.05f;
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
                var shake = Vector3.Lerp(_cameraTransform.position, _cameraTransform.position + shakeVec,
                    Time.deltaTime * 50.0f);
                _cameraTransform.position = shake;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}