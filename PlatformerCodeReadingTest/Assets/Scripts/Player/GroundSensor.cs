using UnityEngine;

namespace SchoolEscape.Player
{
    public sealed class GroundSensor : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _groundMask;
        [SerializeField]
        private Vector2 _boxSize = new Vector2(0.65f, 0.12f);

        public bool IsGrounded { get; private set; }

        private void FixedUpdate()
        {
            IsGrounded = Physics2D.OverlapBox(transform.position, _boxSize, 0f, _groundMask) != null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, _boxSize);
        }
    }
}
