using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    private PlayerController _controller;
    private Animator _animator;

    private static readonly int ForwardToMovementAngle = Animator.StringToHash("forwardToMovementAngle");
    private static readonly int SpeedPercent = Animator.StringToHash("speedPercent");

    private float _forwardToMovementAngle;
    private float _speedPrecent;

    private void Awake()
    {
        _controller ??= GetComponent<PlayerController>();
        _animator ??= GetComponent<Animator>();
    }

    private void Update()
    {
        _forwardToMovementAngle = Mathf.Lerp(_forwardToMovementAngle, _controller.ForwardToMovementAngle(),
            Time.deltaTime * 5.0f);
        _animator.SetFloat(ForwardToMovementAngle, _forwardToMovementAngle);

        _speedPrecent = Mathf.Lerp(_speedPrecent, _controller.SpeedPercent(),
            Time.deltaTime * 10.0f);
        _animator.SetFloat(SpeedPercent, _speedPrecent);
    }
}