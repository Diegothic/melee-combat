using Character;
using ScriptableObjects.Actions.Attacks;
using UnityEngine;

namespace ScriptableObjects.Brains
{
    [CreateAssetMenu(menuName = "Brains/AI")]
    public class AIBrain : HumanoidBrain
    {
        [SerializeField]
        private float awayDistance;
        [SerializeField]
        private float stoppingDistance;
        [SerializeField]
        private float retreatDistance;
        [SerializeField]
        private float minRetreatDistance;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float counterAttackChance;

        private TargetInfo _selfInfo;
        private TargetInfo _targetInfo;

        private bool _didCombatAction;

        public override void Think()
        {
            _selfInfo ??= Controller.Info;
            if (Targeting.HasTarget())
            {
                if (Targeting.TargetInfo != null)
                    _targetInfo = Targeting.TargetInfo;
                CombatState();
            }
            else
            {
                PatrollingState();
            }
        }

        private void CombatState()
        {
            if (_targetInfo == null) return;

            var position = Controller.GetPosition();
            var distanceToTarget = Vector3.Distance(position, _targetInfo.Position);
            if (distanceToTarget < minRetreatDistance)
            {
                InVeryCloseToTargetRange();
                return;
            }

            if (distanceToTarget < retreatDistance)
            {
                InCloseToTargetRange();
                return;
            }

            if (distanceToTarget < stoppingDistance)
            {
                InCombatRange();
                return;
            }

            if (distanceToTarget < awayDistance)
            {
                InAwayFromTargetRange();
                return;
            }

            InFarFromTargetRange();
        }

        private void InVeryCloseToTargetRange()
        {
            if (Controls.IsInRoutine || Controls.IsInAction) return;

            var chance = Random.Range(0.0f, 1.0f);
            if (chance > 0.5f)
            {
                Controls.ForceRetreat();
            }
            else
            {
                Controls.DodgeBackwards();
            }
        }

        private void InCloseToTargetRange()
        {
            CombatBehavior();
            if (_didCombatAction) return;

            if (Controls.IsInRoutine || Controls.IsInAction) return;

            var chance = Random.Range(0.0f, 1.0f);
            if (chance > 0.5)
            {
                Controls.Retreat();
            }
            else if (chance > 0.25f)
            {
                Controls.Strafe(true);
            }
            else
            {
                Controls.Strafe(false);
            }
        }

        private void InCombatRange()
        {
            CombatBehavior();
            if (_didCombatAction) return;

            if (Controls.IsInRoutine || Controls.IsInAction) return;

            var chance = Random.Range(0.0f, 1.0f);
            if (chance > 0.7f)
            {
                Controls.Wait();
            }
            else if (chance > 0.5f)
            {
                Controls.Strafe(true);
            }
            else if (chance > 0.3f)
            {
                Controls.Strafe(false);
            }
            else if (chance > 0.15f)
            {
                Controls.WalkTowardsTarget();
            }
            else
            {
                Controls.Retreat();
            }
        }

        private void InAwayFromTargetRange()
        {
            if (Controls.IsInRoutine || Controls.IsInAction) return;

            var chance = Random.Range(0.0f, 1.0f);
            if (chance > 0.5)
            {
                Controls.WalkTowardsTarget();
            }
            else if (chance > 0.25f)
            {
                Controls.Strafe(true);
            }
            else
            {
                Controls.Strafe(false);
            }
        }

        private void InFarFromTargetRange()
        {
            if (Controls.IsInRoutine || Controls.IsInAction) return;

            var chance = Random.Range(0.0f, 1.0f);
            if (chance > 0.3f)
            {
                Controls.WalkTowardsTarget();
            }
            else
            {
                Controls.DodgeForward();
            }
        }

        private void CombatBehavior()
        {
            var position = Controller.GetPosition();
            var distanceToTarget = Vector3.Distance(position, _targetInfo.Position);
            _didCombatAction = false;

            if (_selfInfo.IsBlocking && (!_targetInfo.IsInAction || _selfInfo.Stamina < 20.0f))
            {
                Controls.StopBlocking();
                return;
            }

            if (_selfInfo.IsInAction && !_selfInfo.IsInCombo)
                return;

            if (_selfInfo.Stamina < 40.0f && Random.Range(0.0f, 1.0f) > 0.5f)
                return;

            if (distanceToTarget <= 1.5f && _targetInfo.IsCloseToAttack && _targetInfo.CurrentAttack != null)
            {
                var counterChance = Random.Range(0.0f, 1.0f);
                if (counterChance > 1.0f - counterAttackChance)
                {
                    if (_targetInfo.CurrentAttack.type == AttackType.Heavy)
                        Controls.HeavyAttack(1.0f);
                    else
                        Controls.LightAttack();
                    _didCombatAction = true;
                    return;
                }
            }

            if (!_targetInfo.IsInAction && Random.Range(0.0f, 1.0f) > 0.7f)
                return;

            if (_targetInfo.IsInAction && _targetInfo.CurrentAttack != null && Random.Range(0.0f, 1.0f) > 0.9f)
            {
                if (_selfInfo.Stamina > 30.0f)
                {
                    if (distanceToTarget < 2.7f && Random.Range(0.0f, 1.0f) > 0.6f)
                    {
                        Controls.DodgeBackwards();
                        _didCombatAction = true;
                        return;
                    }

                    if (!_selfInfo.IsBlocking)
                    {
                        Controls.StartBlocking();
                        _didCombatAction = true;
                        return;
                    }
                }

                if (distanceToTarget < 1.7f && Random.Range(0.0f, 1.0f) > 0.9f)
                {
                    Controls.DodgeBackwards();
                    _didCombatAction = true;
                    return;
                }

                if (distanceToTarget < 2.6f && Random.Range(0.0f, 1.0f) > 0.9f)
                {
                    Controls.DodgeLeft();
                    _didCombatAction = true;
                    return;
                }

                if (distanceToTarget < 2.6f && Random.Range(0.0f, 1.0f) > 0.9f)
                {
                    Controls.DodgeRight();
                    _didCombatAction = true;
                    return;
                }
            }

            if (!_selfInfo.IsInAction && !_selfInfo.IsBlocking && _targetInfo.IsInAction &&
                _targetInfo.CurrentAttack != null)
            {
                var chance = Random.Range(0.0f, 1.0f);
                if (chance > 0.8f && _selfInfo.Stamina > 60)
                {
                    Controls.StartBlocking();
                    _didCombatAction = true;
                    return;
                }
            }

            if (!_selfInfo.IsBlocking)
            {
                if (_selfInfo.IsInCombo && _selfInfo.CurrentAttack != null &&
                    (_selfInfo.CurrentAttack.nextHeavy != null ||
                     _selfInfo.CurrentAttack.nextLight != null))
                {
                    var comboChance = Random.Range(0.0f, 1.0f);
                    if (comboChance > 0.3f)
                    {
                        var comboTypeChance = Random.Range(0.0f, 1.0f);
                        if (comboTypeChance > 0.5f)
                        {
                            if (_selfInfo.CurrentAttack.nextHeavy != null)
                                Controls.HeavyAttack(Random.Range(0.0f, 1.0f));
                            else
                                Controls.LightAttack();
                        }
                        else
                        {
                            if (_selfInfo.CurrentAttack.nextLight != null)
                                Controls.LightAttack();
                            else
                                Controls.HeavyAttack(Random.Range(0.0f, 1.0f));
                        }

                        _didCombatAction = true;
                        return;
                    }
                }

                if (Random.Range(0.0f, 1.0f) > 0.05f)
                    return;

                var chance = Random.Range(0.0f, 1.0f);
                if (chance > 0.6f)
                    Controls.HeavyAttack(Random.Range(0.0f, 1.0f));
                else
                    Controls.LightAttack();
                _didCombatAction = true;
            }
        }

        private void PatrollingState()
        {
            Controller.EnableWalk();
            Controls.WalkRandomly();
        }
    }
}