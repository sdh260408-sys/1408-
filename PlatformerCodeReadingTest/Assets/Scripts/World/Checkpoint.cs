using SchoolEscape.Core;
using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.World
{
    public sealed class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private RespawnManager _respawnManager;
        [SerializeField]
        private SpriteRenderer _indicator;
        [SerializeField]
        private Color _activatedColor = new Color(0.2f, 1f, 0.5f);

        private bool _isActivated;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isActivated || !other.TryGetComponent(out PlayerLife _))
            {
                return;
            }

            _isActivated = true;
            _respawnManager.SetCheckpoint(transform.position + Vector3.up);
            _indicator.color = _activatedColor;
        }
    }
}
