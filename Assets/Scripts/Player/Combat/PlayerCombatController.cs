using Character.Combat;
using Character.Controller;
using Input;
using Player.Controller;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Combat
{
    public class PlayerCombatController : MonoBehaviour, ICombatInputReceiver
    {
        private IPlayerInput _playerInput;
        private ICharacterController _characterController;
        private ICharacterCombat _combat;

        private void Awake()
        {
            _playerInput ??= GetComponent<IPlayerInput>();
            _characterController ??= GetComponent<PlayerController>();
            _combat ??= GetComponent<CharacterCombat>();
        }

        private void ForceCombatState()
        {
            if (_characterController.IsInCombat())
                return;
            transform.eulerAngles = Vector3.up * _characterController.AngleToTarget();
            _characterController.SetState(CharacterState.Combat);
        }

        public void OnLightAttack(InputAction.CallbackContext ctx)
        {
            ForceCombatState();
            _combat.LightAttack();
        }

        public void OnHeavyAttackPress(InputAction.CallbackContext ctx)
        {
            ForceCombatState();
            _combat.StartHeavyAttack();
        }

        public void OnHeavyAttackRelease(InputAction.CallbackContext ctx)
        {
            ForceCombatState();
            _combat.HeavyAttack();
        }

        public void OnBlockPress(InputAction.CallbackContext ctx)
        {
            ForceCombatState();
            _combat.StartBlocking();
        }

        public void OnBlockRelease(InputAction.CallbackContext ctx)
        {
            ForceCombatState();
            _combat.StopBlocking();
        }

        public void OnDodge(InputAction.CallbackContext ctx)
        {
            ForceCombatState();
            _combat.Dodge(AngleToInput());
        }

        private float AngleToInput()
        {
            var input = _playerInput.InputAxis();
            return Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        }
    }
}