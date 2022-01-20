using UnityEngine;

namespace Util
{
    public class RootMotionStabilizer : MonoBehaviour
    {
        private Animator _animator;
        private Transform _transform;
        private Rigidbody _rigidbody;

        private float _yAdjustment;

        private void Awake()
        {
            _animator ??= GetComponent<Animator>();
            _transform ??= GetComponent<Transform>();
            _rigidbody ??= GetComponent<Rigidbody>();

            _yAdjustment = _transform.position.y;
        }

        private void Update()
        {
            if (_animator.applyRootMotion && _rigidbody.velocity.y >= 0.0f)
                AdjustYPosition();
        }

        private void AdjustYPosition()
        {
            var diff = _transform.position.y - _yAdjustment;
            _transform.Translate(0.0f, -diff, 0.0f);
            _yAdjustment = _transform.position.y;
        }
    }
}