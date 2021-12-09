using System;
using Character.Combat;
using Character.Controller;
using Player.Controller;
using UnityEngine;

namespace Character.Animation
{
    public class CharacterCombatAnimator : MonoBehaviour
    {
        private ICharacterController _characterController;
        private ICharacterCombat _combat;
        private Animator _animator;

        private void Awake()
        {
            _characterController ??= GetComponent<PlayerController>();
            _combat ??= GetComponent<CharacterCombat>();
            _animator ??= GetComponent<Animator>();
        }

        public void TriggerLightAttack()
        {
            throw new NotImplementedException();
        }
    }
}