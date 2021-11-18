public interface IMovement
{
    void Move();
    void Rotate();

    float SpeedPercent();
    float ForwardToMovementAngle();
}