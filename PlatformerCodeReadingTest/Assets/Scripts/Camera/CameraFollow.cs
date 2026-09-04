using UnityEngine;

namespace SchoolEscape.CameraSystem
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private float _minX = 0f;
        [SerializeField]
        private float _maxX = 38f;
        [SerializeField, Min(0f)]
        private float _smoothTime = 0.18f;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            float x = Mathf.Clamp(_target.position.x, _minX, _maxX);
            Vector3 desired = new Vector3(x, 0f, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, _smoothTime);
        }
    }
}
