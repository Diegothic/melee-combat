using UnityEngine;
using Util;

namespace Character.LookDirection
{
    public class TransformLookDirection : ILookDirection
    {
        private readonly Transform _transform;

        public TransformLookDirection(Transform transform)
        {
            _transform = transform;
        }

        public Vector3 Right()
        {
            return _transform.right.Flat();
        }

        public Vector3 Forward()
        {
            return _transform.forward.Flat();
        }
    }
}