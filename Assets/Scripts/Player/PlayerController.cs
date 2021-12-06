using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
    private InputAction _movementAxis;

    [SerializeField] private CharacterState state;

    private IMovement _movement;
    private bool _shouldWalk;

    private void Awake()
    {
        _input = new PlayerInput();
        _movement = GetComponent<PlayerMovement>();
        state = CharacterState.Exploration;
    }

    private void OnEnable()
    {
        _movementAxis = _input.Player.Movement;
        _movementAxis.Enable();

        _input.Player.SwitchState.performed += SwitchState;
        _input.Player.SwitchState.Enable();

        _input.Player.EnableWalk.performed += EnableWalk;
        _input.Player.EnableWalk.canceled += DisableWalk;
        _input.Player.EnableWalk.Enable();
    }

    private void OnDisable()
    {
        _movementAxis.Disable();
        _input.Player.SwitchState.Disable();
        _input.Player.EnableWalk.Disable();
    }

    private void SwitchState(InputAction.CallbackContext ctx)
    {
        state = state == CharacterState.Exploration ? CharacterState.Combat : CharacterState.Exploration;
    }

    private void EnableWalk(InputAction.CallbackContext ctx)
    {
        _shouldWalk = true;
    }

    private void DisableWalk(InputAction.CallbackContext ctx)
    {
        _shouldWalk = false;
    }

    public bool ShouldWalk()
    {
        return _shouldWalk;
    }

    public bool IsInCombat()
    {
        return state == CharacterState.Combat;
    }

    private void FixedUpdate()
    {
        _movement.Move();
    }

    private void LateUpdate()
    {
        _movement.Rotate();
    }

    public Vector2 InputAxis()
    {
        return _movementAxis.ReadValue<Vector2>();
    }
}