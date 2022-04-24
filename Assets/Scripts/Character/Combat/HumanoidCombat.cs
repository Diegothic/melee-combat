using System;
using System.Collections;
using Character.Audio;
using ScriptableObjects.Actions.Attacks;
using ScriptableObjects.CombatOptions;
using UnityEngine;

namespace Character.Combat
{
    public class HumanoidCombat : MonoBehaviour
    {
        public event Action<bool> OnInAttackChanged = delegate { };
        public event Action<bool> OnUnblockableChanged = delegate { };

        public event Action OnAttack = delegate { };
        public event Action OnOutOfStamina = delegate { };

        private static readonly int AInAction = Animator.StringToHash("isInAction");
        private static readonly int AIsHeavyReady = Animator.StringToHash("isHeavyReady");
        private static readonly int ADodgeAngle = Animator.StringToHash("dodgeAngle");
        private static readonly int AIsBlocking = Animator.StringToHash("isBlocking");
        private static readonly int ABlockingTrigger = Animator.StringToHash("blockingTrigger");
        private static readonly int AParryTrigger = Animator.StringToHash("parryTrigger");

        [SerializeField]
        private CombatOptions combatOptions;

        [SerializeField]
        private bool isUsingInput;

        public bool IsInAction { get; private set; }
        public bool IsInCombo { get; private set; }
        public bool IsCloseToAttack { get; private set; }
        public bool IsBlocking { get; private set; }
        public bool IsInvincible { get; private set; }
        public bool IsInterruptable { get; private set; }
        public Attack CurrentAttack { get; private set; }

        private bool _isHeavyCharged;
        private bool _isHeavyFullyCharged;
        private bool _isHeavyReleased;

        private bool _isRotatingToTarget;
        private bool _useLookDirection;

        private bool _requestBlocking;
        private bool _shouldBeBlocking;

        private ActionType _lastRequestedAction;
        private float _rememberActionPressCooldown;
        private float _rememberDodgeAngle;

        [SerializeField]
        private ParticleSystem bloodParticles;

        private Transform _transform;
        private Animator _animator;
        private Movement _movement;
        private Targeting _targeting;
        private Attributes _attributes;
        private HumanoidAudio _audio;

        private void Awake()
        {
            IsInAction = false;
            IsInCombo = false;
            IsCloseToAttack = false;
            IsInvincible = false;
            IsInterruptable = true;

            _transform ??= GetComponent<Transform>();
            _animator ??= GetComponent<Animator>();
            _movement ??= GetComponent<Movement>();
            _targeting ??= GetComponent<Targeting>();
            _attributes ??= GetComponent<Attributes>();
            _audio ??= GetComponent<HumanoidAudio>();
        }

        private void Update()
        {
            _animator.SetBool(AInAction, IsInAction);
            _animator.SetBool(AIsHeavyReady, _isHeavyCharged && _isHeavyReleased);
            _animator.SetBool(AIsBlocking, _shouldBeBlocking);

            if (_isRotatingToTarget)
                RotateToTarget();

            if (_shouldBeBlocking && !IsBlocking)
                StartBlocking();

            if (!_shouldBeBlocking && IsBlocking && !IsInAction)
                StopBlocking();

            if (isUsingInput)
                TryToRepeatAction();
        }

        private void RotateToTarget(bool instant = false)
        {
            var speed = instant ? 1_000.0f : 1.0f;
            _movement.InstantRotate(_targeting.Target, _useLookDirection, speed);
        }

        private void TryToRepeatAction()
        {
            if (_rememberActionPressCooldown <= 0.0f)
                return;
            _rememberActionPressCooldown -= Time.deltaTime;

            if (IsInAction) return;
            switch (_lastRequestedAction)
            {
                case ActionType.LightAttack:
                    LightAttack();
                    ForgetActionPress();
                    break;
                case ActionType.HeavyAttack:
                    HeavyAttack();
                    ForgetActionPress();
                    break;
                case ActionType.Block:
                    StartBlocking();
                    ForgetActionPress();
                    break;
                case ActionType.Dodge:
                    Dodge(_rememberDodgeAngle);
                    ForgetActionPress();
                    break;
                default:
                    return;
            }
        }

        private void RememberActionPress(ActionType action)
        {
            _lastRequestedAction = action;
            _rememberActionPressCooldown = combatOptions.rememberActionPressTime;
        }

        private void ForgetActionPress()
        {
            _rememberActionPressCooldown = 0.0f;
        }

        public void OnActionStart()
        {
            IsInAction = true;
            IsBlocking = false;
        }

        public void OnActionFinished()
        {
            ResetState(true, true);
        }

        public void ResetState(bool resetAttack, bool resetBlock)
        {
            _isRotatingToTarget = false;
            _useLookDirection = false;
            ResetRootMotion();

            IsInAction = false;
            IsInvincible = false;
            IsInterruptable = true;
            if (resetAttack)
            {
                IsInCombo = false;
                CurrentAttack = null;
            }

            IsCloseToAttack = false;
            if (resetBlock)
            {
                IsBlocking = false;
            }

            OnInAttackChanged(false);
            OnUnblockableChanged(false);
        }

        private void ResetRootMotion()
        {
            _animator.applyRootMotion = false;
            _movement.MovementEnabled = true;
            _movement.RotationEnabled = true;
        }

        private void SwitchToRootMotion()
        {
            _movement.MovementEnabled = false;
            _movement.RotationEnabled = false;
            _animator.applyRootMotion = true;
        }

        private bool HasEnoughStamina(float requiredStamina)
        {
            if (_attributes.Stamina < requiredStamina)
            {
                OnOutOfStamina();
                return false;
            }

            return true;
        }

        #region Attack

        public void OnAttackStart()
        {
            _isRotatingToTarget = false;
            IsCloseToAttack = true;
            IsInterruptable = false;

            OnInAttackChanged(true);

            _audio.AudioOnAttack();
        }

        public void OnAttackFinished()
        {
            IsInterruptable = true;

            OnInAttackChanged(false);
        }

        public void OnComboStart()
        {
            IsInCombo = true;
        }

        public void OnComboEnd()
        {
            IsInCombo = false;
        }

        public void OnHeavyCharged()
        {
            _isHeavyCharged = true;
        }

        public void OnHeavyFullyCharged()
        {
            _isHeavyReleased = true;
            _isHeavyFullyCharged = true;
        }

        public void OnDamage()
        {
            IsCloseToAttack = false;
            _isHeavyCharged = false;
            _isHeavyReleased = false;

            if (CurrentAttack == null)
                return;

            DealDamage(CurrentAttack.damage,
                CurrentAttack.isUnblockable,
                CurrentAttack.range,
                CurrentAttack.horizontalRange);
        }

        private void DealDamage(float amount, bool unblockable, float range, float horizontalRange)
        {
            if (CurrentAttack != null && CurrentAttack.type == AttackType.Heavy && _isHeavyFullyCharged)
                amount *= 1.2f;
            var damageCenter = _transform.position;
            var candidates = Physics.OverlapSphere(damageCenter,
                range,
                combatOptions.combatLayer);
            foreach (var candidate in candidates)
            {
                if (candidate.gameObject == gameObject)
                    continue;
                var position = candidate.transform.position;
                var damageVector = position - _transform.position;
                var v1 = new Vector2(_transform.forward.x, _transform.forward.z);
                var v2 = new Vector2(damageVector.x, damageVector.z);
                var dot = Vector2.Dot(v1, v2);
                if (dot < 1.0f - horizontalRange)
                    continue;
                var combat = candidate.gameObject.GetComponent<HumanoidCombat>();
                if (combat == null)
                    continue;
                combat.GetHit(amount, unblockable);
                OnAttack();
            }
        }

        public void LightAttack()
        {
            RememberActionPress(ActionType.LightAttack);
            if (IsInAction && !IsInCombo) return;
            ForgetActionPress();

            if (!HasEnoughStamina(combatOptions.lightAttackCost))
                return;
            _attributes.DecreaseStamina(combatOptions.lightAttackCost);

            ResetState(false, true);

            if (!IsInCombo && CheckForCounter(AttackType.Light))
            {
                CounterAttack();
                return;
            }

            if (IsInCombo)
            {
                if (CurrentAttack == null)
                    return;
                if (CurrentAttack.nextLight == null)
                    return;
                CurrentAttack = CurrentAttack.nextLight;
                IsInCombo = false;
            }

            if (CurrentAttack == null)
                CurrentAttack = combatOptions.lightStarter;

            if (CurrentAttack == null)
                return;

            IsCloseToAttack = false;
            _isRotatingToTarget = true;
            SwitchToRootMotion();

            _animator.Play(CurrentAttack.animationName);

            if (CurrentAttack.isUnblockable)
                OnUnblockableChanged(true);
        }

        public void HeavyAttack()
        {
            RememberActionPress(ActionType.HeavyAttack);
            if (IsInAction && !IsInCombo) return;
            ForgetActionPress();

            if (!HasEnoughStamina(combatOptions.heavyAttackCost))
                return;
            _attributes.DecreaseStamina(combatOptions.heavyAttackCost);

            ResetState(false, true);

            if (!IsInCombo && CheckForCounter(AttackType.Heavy))
            {
                CounterAttack();
                return;
            }

            if (IsInCombo)
            {
                if (CurrentAttack == null)
                    return;
                if (CurrentAttack.nextHeavy == null)
                    return;
                CurrentAttack = CurrentAttack.nextHeavy;
                IsInCombo = false;
            }

            if (CurrentAttack == null)
                CurrentAttack = combatOptions.heavyStarter;

            if (CurrentAttack == null)
                return;

            IsCloseToAttack = false;
            _isRotatingToTarget = true;
            SwitchToRootMotion();

            _isHeavyCharged = false;
            _isHeavyFullyCharged = false;
            _isHeavyReleased = false;

            _animator.Play(CurrentAttack.animationName);

            if (CurrentAttack.isUnblockable)
                OnUnblockableChanged(true);
        }

        public void HeavyAttackReady()
        {
            ForgetActionPress();
            _isHeavyReleased = true;
        }

        public void OnCounterDamage()
        {
            DealDamage(20.0f,
                true,
                2.0f,
                1.0f);
        }

        private bool CheckForCounter(AttackType attackType)
        {
            var checkCenter = _transform.position;
            var candidates = Physics.OverlapSphere(checkCenter,
                combatOptions.parryRange,
                combatOptions.combatLayer);
            var countered = false;
            foreach (var candidate in candidates)
            {
                if (candidate.gameObject == gameObject)
                    continue;
                var combat = candidate.gameObject.GetComponent<HumanoidCombat>();
                if (combat == null)
                    continue;
                if (combat.IsCloseToAttack
                    && combat.CurrentAttack != null
                    && combat.CurrentAttack.type == attackType)
                {
                    countered = true;
                    combat.GetCountered(_transform.position);
                }
            }

            return countered;
        }

        private void CounterAttack()
        {
            ForgetActionPress();
            ResetState(true, true);
            RotateToTarget(true);
            SwitchToRootMotion();

            _animator.Play("CounterAttack");
            _audio.AudioOnCounter();
        }

        #endregion

        #region Dodge

        public void OnStartInvincibility()
        {
            IsInvincible = true;
        }

        public void OnEndInvincibility()
        {
            IsInvincible = false;
        }

        public void OnStartInterruptable()
        {
            IsInterruptable = false;
        }

        public void OnEndInterruptable()
        {
            IsInterruptable = true;
        }

        public void Dodge(float angle)
        {
            RememberActionPress(ActionType.Dodge);
            _rememberDodgeAngle = angle;
            if (IsInAction) return;
            ForgetActionPress();

            if (!HasEnoughStamina(combatOptions.dodgeCost))
                return;
            _attributes.DecreaseStamina(combatOptions.dodgeCost);

            ResetState(true, true);

            _isRotatingToTarget = true;
            _useLookDirection = true;
            SwitchToRootMotion();

            _animator.SetFloat(ADodgeAngle, angle);
            _animator.Play("Dodge");
            _audio.AudioOnDodge();
        }

        #endregion

        #region Block

        public void OnBlockTransitionStart()
        {
            IsInAction = true;
        }

        public void OnBlockTransitionFinished()
        {
            ResetState(true, false);
        }

        public void RequestStartBlocking()
        {
            _requestBlocking = true;
            _shouldBeBlocking = true;
        }

        private void StartBlocking()
        {
            if (IsInAction) return;
            ForgetActionPress();

            IsBlocking = true;

            if (!_requestBlocking)
                return;

            if (!HasEnoughStamina(combatOptions.blockCost))
                return;
            _attributes.DecreaseStamina(combatOptions.blockCost);

            ResetState(true, false);

            _requestBlocking = false;
            SwitchToRootMotion();

            if (CheckForParry())
            {
                StartCoroutine(CParryInvincibility());
                _animator.SetTrigger(AParryTrigger);
                _audio.AudioOnParry();
            }
            else
            {
                _animator.SetTrigger(ABlockingTrigger);
                _audio.AudioOnStartBlocking();
            }
        }

        private IEnumerator CParryInvincibility()
        {
            IsInvincible = true;
            const float time = 0.2f;
            var elapsedTime = 0.0f;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            IsInvincible = false;
        }

        private bool CheckForParry()
        {
            var checkCenter = _transform.position;
            var candidates = Physics.OverlapSphere(checkCenter,
                combatOptions.parryRange,
                combatOptions.combatLayer);
            foreach (var candidate in candidates)
            {
                if (candidate.gameObject == gameObject)
                    continue;
                var combat = candidate.gameObject.GetComponent<HumanoidCombat>();
                if (combat == null)
                    continue;
                if (combat.IsCloseToAttack && combat.CurrentAttack != null)
                    return true;
            }

            return false;
        }

        public void RequestStopBlocking()
        {
            _requestBlocking = false;
            _shouldBeBlocking = false;
        }

        private void StopBlocking()
        {
            IsBlocking = false;
        }

        #endregion

        public void GetHit(float amount, bool unblockable)
        {
            if (IsInvincible) return;

            ForgetActionPress();
            if (!IsInterruptable)
            {
                _audio.AudioOnSwordsHit();
                bloodParticles.Play();
                _attributes.DecreaseHealth(amount);
                StartCoroutine(CKnockBack(0.2f, 7.0f));
                return;
            }

            if (IsBlocking && !unblockable)
            {
                _audio.AudioOnBlock();
                _attributes.DecreaseHealth(amount * combatOptions.chipDamageMultiplier);
                StartCoroutine(CKnockBack(0.1f, 5.0f));
                BlockedAttack();
                return;
            }

            _audio.AudioOnGetHit();
            bloodParticles.Play();
            _attributes.DecreaseHealth(amount);
            StartCoroutine(CKnockBack(0.1f, 5.0f));
            GotDamaged();
        }

        private IEnumerator CKnockBack(float time, float strength)
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < time)
            {
                _transform.position -= _transform.forward * strength * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private void BlockedAttack()
        {
            ResetState(true, false);
            RotateToTarget(true);
            SwitchToRootMotion();
            _animator.Play("BlockAttack");
        }

        private void GotDamaged()
        {
            ResetState(true, true);
            RotateToTarget(true);
            SwitchToRootMotion();
            _animator.Play("GetHurt");
        }

        public void GetCountered(Vector3 countererPos)
        {
            ForgetActionPress();
            ResetState(true, true);
            RotateToTarget(true);
            SwitchToRootMotion();
            StartCoroutine(CCounterFixPosition(countererPos, 0.05f));
            _animator.Play("GetCountered");
        }

        private IEnumerator CCounterFixPosition(Vector3 position, float time)
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < time)
            {
                var betweenVec = (_transform.position - position).normalized;
                var newPos = Vector3.Lerp(_transform.position, position + betweenVec, elapsedTime / time);
                _transform.position = newPos;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, combatOptions.parryRange);
        }
    }
}