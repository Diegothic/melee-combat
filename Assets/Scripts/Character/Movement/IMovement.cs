namespace Character.Movement
{
    public interface IMovement
    {
        void Enable();
        void Disable();

        bool IsMoving();

        float SpeedPercent();
    }
}