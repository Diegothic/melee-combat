using UnityEngine;

namespace Character.Animations
{
    public class MovementAnimator : MonoBehaviour
    {
        private static readonly int ForwardToMovementAngle = Animator.StringToHash("forwardToMovementAngle");
        private static readonly int SpeedPercent = Animator.StringToHash("speedPercent");
        private static readonly int IsInCombat = Animator.StringToHash("isInCombat");

        private float _forwardToMovementAngle;
        private float _speedPercent;

        private Animator _animator;

        private void Awake()
        {
            _animator ??= GetComponent<Animator>();
        }

        public void SetForwardToMovementAngle(float angle)
        {
            _forwardToMovementAngle = Mathf.LerpAngle(_forwardToMovementAngle,
                angle + 180.0f,
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

        public void SetSpeedPercent(float speedPercent)
        {
            _speedPercent = Mathf.Lerp(_speedPercent, speedPercent,
                Time.deltaTime * 10.0f);
            _animator.SetFloat(SpeedPercent, _speedPercent);
        }

        public void SetInCombat(bool inCombat)
        {
            _animator.SetBool(IsInCombat, inCombat);
        }
    }
}