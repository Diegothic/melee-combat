using System;
using Character.Combat;
using ScriptableObjects.Attributes;
using UnityEngine;

namespace Character
{
    public class Attributes : MonoBehaviour
    {
        public event Action<float, float> OnHealthChanged = delegate { };
        public event Action<float, float> OnStaminaChanged = delegate { };

        [SerializeField]
        private BaseAttributes baseAttributes;

        public float Health { get; private set; }
        public float Stamina { get; private set; }

        public bool RegenerationEnabled { get; private set; }

        private float _staminaRegenSpeed;

        private HumanoidCombat _combat;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            Health = baseAttributes.health;
            Stamina = baseAttributes.stamina;

            RegenerationEnabled = true;
            _staminaRegenSpeed = baseAttributes.staminaRegenSpeed;

            _combat ??= GetComponent<HumanoidCombat>();
            _rigidbody ??= GetComponent<Rigidbody>();
        }

        private void Update()
        {
            CorrectStaminaRegenSpeed();
            RegenerateStamina();
        }

        private void CorrectStaminaRegenSpeed()
        {
            if (_combat == null || _rigidbody == null) return;

            if (_combat.IsInAction || _combat.IsBlocking)
            {
                _staminaRegenSpeed = 0.0f;
                return;
            }

            if (_rigidbody.velocity.magnitude > float.Epsilon)
            {
                _staminaRegenSpeed = baseAttributes.staminaRegenSpeed * baseAttributes.inMovementRegenModifier;
                return;
            }

            _staminaRegenSpeed = baseAttributes.staminaRegenSpeed;
        }

        private void RegenerateStamina()
        {
            if (!RegenerationEnabled) return;

            Stamina += _staminaRegenSpeed * Time.deltaTime;

            if (Stamina >= baseAttributes.stamina)
                Stamina = baseAttributes.stamina;

            OnStaminaChanged(Stamina, baseAttributes.stamina);
        }

        public void DecreaseHealth(float amount)
        {
            Health -= amount;
            if (Health <= 0.0f)
                Health = 0.0f;
            OnHealthChanged(Health, baseAttributes.health);
        }

        public void DecreaseStamina(float amount)
        {
            Stamina -= amount;
            if (Stamina <= 0.0f)
                Stamina = 0.0f;
            OnStaminaChanged(Stamina, baseAttributes.stamina);
        }
    }
}