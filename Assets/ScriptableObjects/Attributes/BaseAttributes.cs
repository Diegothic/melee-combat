using UnityEngine;

namespace ScriptableObjects.Attributes
{
    [CreateAssetMenu(menuName = "BaseAttributes")]
    public class BaseAttributes : ScriptableObject
    {
        public float health;
        public float stamina;
        public float staminaRegenSpeed;

        public float inMovementRegenModifier;
    }
}