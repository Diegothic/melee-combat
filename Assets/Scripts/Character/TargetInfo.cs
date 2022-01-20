using ScriptableObjects.Actions.Attacks;
using UnityEngine;

namespace Character
{
    public class TargetInfo
    {
        public Vector3 Position { get; set; }
        public bool IsInCombat { get; set; }

        public bool IsInAction { get; set; }
        public bool IsInCombo { get; set; }
        public bool IsCloseToAttack { get; set; }
        public bool IsBlocking { get; set; }
        public bool IsInvincible { get; set; }
        public bool IsInterruptable { get; set; }
        public Attack CurrentAttack { get; set; }

        public float Health { get; set; }
        public float Stamina { get; set; }

        public TargetInfo()
        {
            Position = new Vector3();
            IsInCombat = false;
            IsInAction = false;
            IsInCombo = false;
            IsCloseToAttack = false;
            IsBlocking = false;
            IsInvincible = false;
            IsInterruptable = true;
            CurrentAttack = null;
        }
    }
}