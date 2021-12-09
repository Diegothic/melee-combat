using System;
using Character.Animation;
using Character.Controller;
using UnityEngine;

namespace Character.Combat
{
    public class CharacterCombat : MonoBehaviour, ICharacterCombat
    {
        private bool _canAttack;
        private bool _isInCombo;
        private bool _canCancel;
        private bool _canBeInterrupted;

        private ICharacterController _characterController;
        private CharacterCombatAnimator _combatAnimator;

        private void Awake()
        {
            _canAttack = true;

            _characterController ??= GetComponent<ICharacterController>();
            _combatAnimator ??= GetComponent<CharacterCombatAnimator>();
        }

        public void Reset()
        {
            _characterController.EnableMovement();
        }

        public void LightAttack()
        {
            if (!_canAttack)
                return;
            _characterController.DisableMovement();
            _combatAnimator.TriggerLightAttack();
        }

        public void StartHeavyAttack()
        {
            throw new NotImplementedException();
        }

        public void HeavyAttack()
        {
            throw new NotImplementedException();
        }

        public void StartBlocking()
        {
            throw new NotImplementedException();
        }

        public void StopBlocking()
        {
            throw new NotImplementedException();
        }

        public void Parry()
        {
            throw new NotImplementedException();
        }

        public void Dodge(float direction)
        {
            throw new NotImplementedException();
        }
    }
}