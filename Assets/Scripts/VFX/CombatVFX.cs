using Character.Combat;
using UnityEngine;

namespace VFX
{
    public class CombatVFX : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem weaponTrail;
        [SerializeField]
        private ParticleSystem unblockableEffect;

        private void Awake()
        {
            var combat = GetComponentInParent<HumanoidCombat>();
            if (combat == null)
                return;
            combat.OnInAttackChanged += SetWeaponTrail;
            combat.OnUnblockableChanged += SetUnblockableEffect;
        }

        private void SetWeaponTrail(bool enable)
        {
            if (enable)
            {
                weaponTrail.Play();
                return;
            }

            weaponTrail.Stop();
        }

        private void SetUnblockableEffect(bool enable)
        {
            if (enable)
            {
                unblockableEffect.Play();
                return;
            }

            unblockableEffect.Stop();
        }
    }
}