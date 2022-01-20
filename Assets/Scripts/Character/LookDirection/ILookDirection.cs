using UnityEngine;

namespace Character.LookDirection
{
    public interface ILookDirection
    {
        public Vector3 Right();
        public Vector3 Forward();
    }
}