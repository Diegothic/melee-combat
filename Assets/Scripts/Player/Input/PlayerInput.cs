using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input
{
    public class PlayerInput : MonoBehaviour, IPlayerInput
    {
        private GameInputSchema _input;
        private InputAction _movementAxis;

        private IMovementInputReceiver _movement;
        private ICombatInputReceiver _combat;

        private void Awake()
        {
            _input = new GameInputSchema();

            _movement ??= GetComponent<IMovementInputReceiver>();
            _combat ??= GetComponent<ICombatInputReceiver>();
        }

        private void OnEnable()
        {
            _movementAxis = _input.Player.Movement;
            _movementAxis.Enable();

            _input.Player.EnableWalk.performed += _movement.EnableWalk;
            _input.Player.EnableWalk.canceled += _movement.DisableWalk;
            _input.Player.EnableWalk.Enable();

            _input.Player.LightAttack.performed += _combat.OnLightAttack;
            _input.Player.LightAttack.Enable();

            _input.Player.HeavyAttack.performed += _combat.OnHeavyAttackPress;
            _input.Player.HeavyAttack.canceled += _combat.OnHeavyAttackRelease;
            _input.Player.HeavyAttack.Enable();

            _input.Player.Block.performed += _combat.OnBlockPress;
            _input.Player.Block.canceled += _combat.OnBlockRelease;
            _input.Player.Block.Enable();

            _input.Player.Dodge.performed += _combat.OnDodge;
            _input.Player.Dodge.Enable();
        }

        private void OnDisable()
        {
            _movementAxis.Disable();
            _input.Player.SwitchState.Disable();
            _input.Player.EnableWalk.Disable();
            _input.Player.LightAttack.Disable();
            _input.Player.HeavyAttack.Disable();
            _input.Player.Block.Disable();
            _input.Player.Dodge.Disable();
        }

        public Vector2 InputAxis()
        {
            return _movementAxis.ReadValue<Vector2>();
        }
    }
}