using Character.Controller;
using Character.Movement;
using Player.Controller;
using Player.Movement;
using UnityEngine;

namespace Character.Animation
{
    public class CharacterAnimator : MonoBehaviour
    {
        private static readonly int ForwardToMovementAngle = Animator.StringToHash("forwardToMovementAngle");
        private static readonly int SpeedPercent = Animator.StringToHash("speedPercent");
        private static readonly int IsInCombat = Animator.StringToHash("isInCombat");

        private float _forwardToMovementAngle;
        private float _speedPercent;

        private ICharacterController _characterController;
        private IMovement _movement;
        private Animator _animator;

        private void Awake()
        {
            _characterController ??= GetComponent<PlayerController>();
            _animator ??= GetComponent<Animator>();
            _movement ??= GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            SetForwardToMovementAngle();
            SetSpeedPercent();
            SetInCombat();
        }

        private void SetForwardToMovementAngle()
        {
            _forwardToMovementAngle = Mathf.LerpAngle(_forwardToMovementAngle,
                _characterController.ForwardToMovementAngle() + 180.0f,
                0.3f);
            _animator.SetFloat(ForwardToMovementAngle, WrapAngle(_forwardToMovementAngle - 180.0f));
        }

        private static float WrapAngle(float angle)
        {
            var result = angle;
            while (!(result >= -180.0f && result <= 180.0f))
            {
                if (result > 180.0f)
                {
                    result -= 360.0f;
                }

                if (result < -180.0f)
                {
                    result += 360.0f;
                }
            }

            return result;
        }

        private void SetSpeedPercent()
        {
            _speedPercent = Mathf.Lerp(_speedPercent, _movement.SpeedPercent(),
                Time.deltaTime * 10.0f);
            _animator.SetFloat(SpeedPercent, _speedPercent);
        }

        private void SetInCombat()
        {
            _animator.SetBool(IsInCombat, _characterController.IsInCombat());
        }
    }
}