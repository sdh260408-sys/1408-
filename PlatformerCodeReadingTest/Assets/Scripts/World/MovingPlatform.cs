using UnityEngine;

namespace SchoolEscape.World
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MovingPlatform : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _movement = new Vector2(3f, 0f);
        [SerializeField, Min(0.1f)]
        private float _duration = 2f;

        private Rigidbody2D _body;
        private Vector2 _startPosition;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _startPosition = _body.position;
        }

        private void FixedUpdate()
        {
            float progress = (Mathf.Sin(Time.time * Mathf.PI / _duration) + 1f) * 0.5f;
            _body.MovePosition(_startPosition + _movement * progress);
        }
    }
}
