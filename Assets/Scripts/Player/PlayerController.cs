using UnityEngine;

[RequireComponent(typeof(IMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterState state;

    private IMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();

        state = CharacterState.Exploration;
    }

    public bool IsInCombat()
    {
        return state == CharacterState.Combat;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchState();
        }
    }

    private void FixedUpdate()
    {
        _movement.Move();
    }

    private void LateUpdate()
    {
        _movement.Rotate();
    }

    private void SwitchState()
    {
        state = state == CharacterState.Exploration ? CharacterState.Combat : CharacterState.Exploration;
    }

    public static Vector2 InputAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}