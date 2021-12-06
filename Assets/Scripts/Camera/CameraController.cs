using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 combatOffset;
    [SerializeField] private float followSpeed;

    private Transform _transform;
    private Transform _target;
    private PlayerController _playerController;
    private ICameraRotator _cameraRotator;
    private CameraTransitionController _transitionController;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        _target ??= playerObject.transform;
        _playerController ??= playerObject.GetComponent<PlayerController>();
        _cameraRotator ??= GetComponent<ICameraRotator>();
        _transitionController ??= GetComponent<CameraTransitionController>();
    }

    public Vector3 FlatForwardVector()
    {
        return FlattenVector(_transform.forward);
    }

    public Vector3 FlatRightVector()
    {
        return FlattenVector(_transform.right);
    }

    private static Vector3 FlattenVector(Vector3 vector)
    {
        var result = vector;
        result.y = 0;
        return result.normalized;
    }

    private void Update()
    {
        _transitionController.UpdateDesiredPosition(CalculateDesiredPosition());
    }

    private void LateUpdate()
    {
        _transitionController.CheckForTransition();
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        if (_transitionController.IsTransitioning())
        {
            return;
        }

        transform.eulerAngles = _cameraRotator.RotateCamera();
        FollowTarget();
    }

    private void FollowTarget()
    {
        transform.position = Vector3.Lerp(transform.position, CalculateDesiredPosition(), followSpeed);
    }

    private Vector3 CalculateDesiredPosition()
    {
        var selectedOffset = _playerController.IsInCombat() ? combatOffset : offset;

        var desiredPosition = _target.position - _transform.forward * selectedOffset.z +
                              _transform.right * selectedOffset.x;
        desiredPosition.y += selectedOffset.y;
        return desiredPosition;
    }
}