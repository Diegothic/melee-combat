using UnityEngine;

namespace Util
{
    public static class Vector3Util
    {
        public static Vector3 Flat(this Vector3 vector)
        {
            var result = new Vector3(vector.x, 0.0f, vector.z);
            return result.normalized;
        }
    }
}