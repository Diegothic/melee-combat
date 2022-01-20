using UnityEngine;

namespace UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private void Update()
        {
            var cam = UnityEngine.Camera.main;
            if (cam == null)
                return;
            var rotation = Quaternion.LookRotation(-cam.transform.forward, Vector3.up);
            transform.rotation = rotation;
        }
    }
}