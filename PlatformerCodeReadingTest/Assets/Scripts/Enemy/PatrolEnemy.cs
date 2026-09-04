using UnityEngine;

namespace SchoolEscape.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PatrolEnemy : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float _speed = 2f;
        [SerializeField, Min(0f)]
        private float _patrolDistance = 2f;

        private Rigidbody2D _body;
        private float _startX;
        private float _direction = -1f;
        private bool _canMove = true;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _startX = transform.position.x;
        }

        private void FixedUpdate()
        {
            if (!_canMove)
            {
                return;
            }

            if (Mathf.Abs(_body.position.x - _startX) >= _patrolDistance)
            {
                _direction = -Mathf.Sign(_body.position.x - _startX);
            }

            _body.velocity = new Vector2(_direction * _speed, _body.velocity.y);
        }

        public void StopMoving()
        {
            _canMove = false;
            _body.velocity = Vector2.zero;
        }
    }
}
