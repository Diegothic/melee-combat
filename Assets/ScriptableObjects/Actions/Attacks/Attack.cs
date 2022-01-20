using UnityEngine;

namespace ScriptableObjects.Actions.Attacks
{
    [CreateAssetMenu(menuName = "Attack")]
    public class Attack : ScriptableObject
    {
        public AttackType type;

        public string animationName;
        public int damage;
        public float range;
        public float horizontalRange;
        public bool isUnblockable;

        public Attack nextLight;
        public Attack nextHeavy;

        public bool IsFinisher()
        {
            return nextLight == null && nextHeavy == null;
        }
    }
}