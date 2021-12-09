using UnityEngine.InputSystem;

namespace Input
{
    public interface ICombatInputReceiver
    {
        void OnLightAttack(InputAction.CallbackContext ctx);
        void OnHeavyAttackPress(InputAction.CallbackContext ctx);
        void OnHeavyAttackRelease(InputAction.CallbackContext ctx);

        void OnBlockPress(InputAction.CallbackContext ctx);
        void OnBlockRelease(InputAction.CallbackContext ctx);

        void OnDodge(InputAction.CallbackContext ctx);
    }
}