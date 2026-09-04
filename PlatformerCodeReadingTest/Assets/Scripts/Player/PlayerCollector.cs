using SchoolEscape.Contracts;
using UnityEngine;

namespace SchoolEscape.Player
{
    public sealed class PlayerCollector : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out ICollectible collectible))
            {
                collectible.Collect(this);
            }
        }
    }
}
