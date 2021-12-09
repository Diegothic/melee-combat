using UnityEngine.InputSystem;

namespace Input
{
    public interface IMovementInputReceiver
    {
        void EnableWalk(InputAction.CallbackContext ctx);
        void DisableWalk(InputAction.CallbackContext ctx);
    }
}