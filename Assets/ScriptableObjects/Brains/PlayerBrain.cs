using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScriptableObjects.Brains
{
    [CreateAssetMenu(menuName = "Brains/Player")]
    public class PlayerBrain : HumanoidBrain
    {
        public static event Action<string> OnInputDeviceChanged = delegate { };

        private GameInputSchema _input;
        private InputAction _movementAxis;

        private Vector2 InputAxis()
        {
            return _movementAxis.ReadValue<Vector2>();
        }

        private float AngleToInput()
        {
            var input = InputAxis();
            return Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        }

        private void OnEnable()
        {
            _input = new GameInputSchema();

            _movementAxis = _input.Player.Movement;
            _movementAxis.started += OnMovement;
            _movementAxis.Enable();

            _input.Player.EnableWalk.performed += EnableWalk;
            _input.Player.EnableWalk.canceled += DisableWalk;
            _input.Player.EnableWalk.Enable();

            _input.Player.LightAttack.started += OnLightAttack;
            _input.Player.LightAttack.Enable();

            _input.Player.HeavyAttack.started += OnHeavyAttackPress;
            _input.Player.HeavyAttack.canceled += OnHeavyAttackRelease;
            _input.Player.HeavyAttack.Enable();

            _input.Player.Block.started += OnBlockPress;
            _input.Player.Block.canceled += OnBlockRelease;
            _input.Player.Block.Enable();

            _input.Player.Dodge.started += OnDodge;
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

        public override void Think()
        {
            Controller.Move(InputAxis());
        }

        private static void OnMovement(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
        }

        private void EnableWalk(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            Controller.EnableWalk();
        }

        private void DisableWalk(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            Controller.DisableWalk();
        }

        private void OnLightAttack(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            Controller.LightAttack();
        }

        private void OnHeavyAttackPress(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            Controller.ChargeHeavyAttack();
        }

        private void OnHeavyAttackRelease(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            Controller.HeavyAttack();
        }

        private void OnBlockPress(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            Controller.Block();
        }

        private void OnBlockRelease(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            Controller.StopBlocking();
        }

        private void OnDodge(InputAction.CallbackContext ctx)
        {
            OnInputDeviceChanged(ctx.control.device.displayName);
            var dodgeAngle = -180.0f;
            if (InputAxis().magnitude > 0.0f)
                dodgeAngle = AngleToInput();
            Controller.Dodge(dodgeAngle);
        }
    }
}