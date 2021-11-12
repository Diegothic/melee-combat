using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ICameraRotator))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    [SerializeField] private float followSpeed;

    private Transform _target;

    private ICameraRotator _cameraRotator;

    private void Awake()
    {
        _target ??= GameObject.FindGameObjectWithTag("Player").transform;
        _cameraRotator ??= GetComponent<ICameraRotator>();
    }

    public Vector3 GetLookForward()
    {
        return FlattenVector(transform.forward);
    }

    public Vector3 GetLookRight()
    {
        return FlattenVector(transform.right);
    }

    private static Vector3 FlattenVector(Vector3 vector)
    {
        var result = vector;
        result.y = 0;
        return result.normalized;
    }

    private void LateUpdate()
    {
        transform.eulerAngles = _cameraRotator.RotateCamera();
        FollowTarget();
    }

    private void FollowTarget()
    {
        var desiredPosition = _target.position - transform.forward * offset.z + transform.right * offset.x;
        desiredPosition.y += offset.y;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed);
    }
}