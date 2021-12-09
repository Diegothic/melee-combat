namespace Character.Controller
{
    public interface ICharacterController
    {
        void EnableMovement();
        void DisableMovement();

        bool IsInCombat();
        void SetState(CharacterState newState);

        float AngleToTarget();
        float AngleToVelocity();
        float ForwardToMovementAngle();
    }
}