using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.World
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Brick : MonoBehaviour
    {
        private Collider2D _brickCollider;

        protected virtual void Awake()
        {
            _brickCollider = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out PlayerMotor playerMotor))
            {
                return;
            }

            if (!HitBottomSurface(collision))
            {
                return;
            }

            OnHitFromBelow(playerMotor);
        }

        protected abstract void OnHitFromBelow(PlayerMotor playerMotor);

        private bool HitBottomSurface(Collision2D collision)
        {
            float bottomY = _brickCollider.bounds.min.y;
            const float contactTolerance = 0.08f;

            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.GetContact(i).point.y <= bottomY + contactTolerance)
                {
                    return true;
                }
            }

            return false;
        }

        protected void DisableCollision()
        {
            _brickCollider.enabled = false;
        }
    }
}
