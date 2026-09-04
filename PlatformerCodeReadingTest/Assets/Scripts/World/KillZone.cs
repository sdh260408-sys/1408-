using SchoolEscape.Contracts;
using UnityEngine;

namespace SchoolEscape.World
{
    public sealed class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(1);
            }
        }
    }
}
