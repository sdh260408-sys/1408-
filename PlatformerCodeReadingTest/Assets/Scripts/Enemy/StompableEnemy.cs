using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.Enemy
{
    public sealed class StompableEnemy : MonoBehaviour
    {
        [SerializeField]
        private PatrolEnemy _patrol;
        [SerializeField]
        private Collider2D _bodyCollider;
        [SerializeField, Min(0f)]
        private float _stompHeight = 0.35f;
        [SerializeField, Min(0f)]
        private float _bounceVelocity = 8f;

        private bool _isDefeated;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isDefeated || !collision.gameObject.TryGetComponent(out PlayerLife playerLife))
            {
                return;
            }

            PlayerMotor playerMotor = collision.gameObject.GetComponent<PlayerMotor>();
            bool playerIsAbove = collision.transform.position.y > transform.position.y + _stompHeight;
            bool playerIsFalling = playerMotor.VerticalVelocity <= 0f;

            if (playerIsAbove && playerIsFalling)
            {
                Defeat(playerMotor);
            }
            else
            {
                playerLife.TakeDamage(1);
            }
        }

        private void Defeat(PlayerMotor playerMotor)
        {
            _isDefeated = true;
            _patrol.StopMoving();
            _bodyCollider.enabled = false;
            transform.localScale = new Vector3(1f, 0.25f, 1f);
            playerMotor.Bounce(_bounceVelocity);
            Destroy(gameObject, 0.35f);
        }
    }
}
