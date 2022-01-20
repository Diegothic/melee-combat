using UnityEngine;

namespace Character.State
{
    public class StateManager
    {
        public CharacterState State { get; private set; }
        private readonly float _stateResetCdBase;
        private float _stateResetCd;

        public StateManager(float resetCooldown)
        {
            State = CharacterState.Exploration;
            _stateResetCdBase = resetCooldown;
        }

        public bool IsCombat()
        {
            return State == CharacterState.Combat;
        }

        public void ForceCombat()
        {
            SetState(CharacterState.Combat);
        }

        public void UpdateState(bool hasTarget, bool walking)
        {
            if (hasTarget)
            {
                SetState(CharacterState.Combat);
                return;
            }

            if (_stateResetCd > 0.0f)
            {
                _stateResetCd -= Time.deltaTime;
                return;
            }

            SetState(walking ? CharacterState.Walk : CharacterState.Exploration);
        }

        private void SetState(CharacterState newState)
        {
            if (newState == CharacterState.Combat)
                _stateResetCd = _stateResetCdBase;
            State = newState;
        }
    }
}