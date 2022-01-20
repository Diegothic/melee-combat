using Character.Animations;
using Character.Audio;
using Character.Combat;
using Character.State;
using ScriptableObjects.Brains;
using UnityEngine;

namespace Character
{
    public class HumanoidController : MonoBehaviour
    {
        public bool IsAlive { get; private set; }

        [SerializeField]
        private HumanoidBrain brain;

        [SerializeField]
        private float stateResetCooldown;
        private StateManager _state;
        private bool _shouldWalk;

        private Rigidbody _rigidbody;
        private Targeting _targeting;
        private Movement _movement;
        private HumanoidCombat _combat;
        private Attributes _attributes;
        private MovementAnimator _movementAnimator;
        private Animator _animator;
        private HumanoidAudio _audio;

        public TargetInfo Info { get; private set; }

        private void Awake()
        {
            IsAlive = true;

            _rigidbody ??= GetComponent<Rigidbody>();
            _targeting ??= GetComponent<Targeting>();
            _movement ??= GetComponent<Movement>();
            _combat ??= GetComponent<HumanoidCombat>();
            _attributes ??= GetComponent<Attributes>();
            _movementAnimator ??= GetComponent<MovementAnimator>();
            _animator ??= GetComponent<Animator>();
            _audio ??= GetComponent<HumanoidAudio>();

            _state = new StateManager(stateResetCooldown);

            var actions = GetComponent<AIControls>();
            brain.Init(this, _targeting, actions);

            Info = new TargetInfo();
        }

        private void Update()
        {
            if (!IsAlive)
                return;

            if (ShouldBeDeadDead())
                Die();

            brain.Think();

            if (_combat.IsBlocking)
                _state.ForceCombat();

            _state.UpdateState(_targeting.HasTarget(), _shouldWalk);
            _movementAnimator.SetInCombat(_state.IsCombat());

            _movement.Rotate(_targeting.Target);

            UpdateInfo();
        }

        private bool ShouldBeDeadDead()
        {
            return _attributes.Health <= 0.0f;
        }

        private void Die()
        {
            IsAlive = false;
            _targeting.Disable();

            _audio.AudioOnDeath();
            _rigidbody.isKinematic = true;
            _animator.applyRootMotion = true;
            _animator.Play("Death");
            GetComponent<CapsuleCollider>().enabled = false;
        }

        private void UpdateInfo()
        {
            Info.Position = transform.position;
            Info.IsInCombat = _state.IsCombat();
            Info.IsInAction = _combat.IsInAction;
            Info.IsInCombo = _combat.IsInCombo;
            Info.IsCloseToAttack = _combat.IsCloseToAttack;
            Info.IsBlocking = _combat.IsBlocking;
            Info.IsInvincible = _combat.IsInvincible;
            Info.IsInterruptable = _combat.IsInterruptable;
            Info.CurrentAttack = _combat.CurrentAttack;
            Info.Health = _attributes.Health;
            Info.Stamina = _attributes.Stamina;
        }

        public bool IsInCombat()
        {
            return _state.IsCombat();
        }

        public void EnableWalk()
        {
            _shouldWalk = true;
        }

        public void DisableWalk()
        {
            _shouldWalk = false;
        }

        public void Move(Vector2 direction)
        {
            if (!IsAlive)
                return;
            _movement.Move(direction, _state.State);
        }

        public void LightAttack()
        {
            if (!IsAlive)
                return;
            _state.ForceCombat();
            _combat.LightAttack();
        }

        public void ChargeHeavyAttack()
        {
            if (!IsAlive)
                return;
            _state.ForceCombat();
            _combat.HeavyAttack();
        }

        public void HeavyAttack()
        {
            if (!IsAlive)
                return;
            _combat.HeavyAttackReady();
        }

        public void Block()
        {
            if (!IsAlive)
                return;
            _state.ForceCombat();
            _combat.RequestStartBlocking();
        }

        public void StopBlocking()
        {
            if (!IsAlive)
                return;
            _combat.RequestStopBlocking();
        }

        public void Dodge(float angle)
        {
            if (!IsAlive)
                return;
            _state.ForceCombat();
            _combat.Dodge(angle);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        private void OnDrawGizmos()
        {
            if (_state == null) return;

            Gizmos.color = _state.IsCombat() ? Color.red : Color.green;
            Gizmos.DrawSphere(transform.position + Vector3.up * 2.1f, 0.05f);
        }
    }
}