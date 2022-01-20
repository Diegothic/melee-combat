using UnityEngine;
using Util;

namespace Character.LookDirection
{
    public class CameraLookDirection : ILookDirection
    {
        private readonly Transform _cameraTransform;

        public CameraLookDirection()
        {
            _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        }

        public Vector3 Right()
        {
            return _cameraTransform.right.Flat();
        }

        public Vector3 Forward()
        {
            return _cameraTransform.forward.Flat();
        }
    }
}