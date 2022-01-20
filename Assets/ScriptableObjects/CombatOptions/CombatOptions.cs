using ScriptableObjects.Actions.Attacks;
using UnityEngine;

namespace ScriptableObjects.CombatOptions
{
    [CreateAssetMenu(menuName = "CombatOptions")]
    public class CombatOptions : ScriptableObject
    {
        [Header("Physics")]
        public LayerMask combatLayer;

        [Header("Actions")]
        public float parryRange;
        public Attack lightStarter;
        public Attack heavyStarter;
        public float rememberActionPressTime;

        [Header("Stamina Cost")]
        public float lightAttackCost;
        public float heavyAttackCost;
        public float dodgeCost;
        public float blockCost;

        [Header("Damage")]
        public float chipDamageMultiplier;
    }
}