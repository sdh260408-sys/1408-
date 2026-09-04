using UnityEngine;

namespace SchoolEscape.Data
{
    [CreateAssetMenu(menuName = "School Escape/Movement Settings")]
    public sealed class MovementSettings : ScriptableObject
    {
        [Header("Horizontal Movement")]
        [SerializeField, Min(0f)] private float _maxSpeed = 7f;
        [SerializeField, Min(0f)] private float _groundAcceleration = 42f;
        [SerializeField, Min(0f)] private float _groundDeceleration = 20f;
        [SerializeField, Min(0f)] private float _turnaroundAcceleration = 95f;
        [SerializeField, Min(0f)] private float _airAcceleration = 26f;
        [SerializeField, Min(0f)] private float _airDeceleration = 8f;

        [Header("Jump")]
        [SerializeField, Min(0f)] private float _jumpVelocity = 13f;
        [SerializeField, Range(0.1f, 1f)] private float _jumpReleaseMultiplier = 0.45f;
        [SerializeField, Min(0f)] private float _coyoteTime = 0.12f;
        [SerializeField, Min(0f)] private float _jumpBufferTime = 0.12f;

        [Header("Gravity")]
        [SerializeField, Min(0f)] private float _baseGravityScale = 3.2f;
        [SerializeField, Min(1f)] private float _lowJumpGravityMultiplier = 2.2f;
        [SerializeField, Min(1f)] private float _fallGravityMultiplier = 1.65f;
        [SerializeField, Min(0f)] private float _maxFallSpeed = 20f;

        public float MaxSpeed => _maxSpeed;
        public float GroundAcceleration => _groundAcceleration;
        public float GroundDeceleration => _groundDeceleration;
        public float TurnaroundAcceleration => _turnaroundAcceleration;
        public float AirAcceleration => _airAcceleration;
        public float AirDeceleration => _airDeceleration;
        public float JumpVelocity => _jumpVelocity;
        public float JumpReleaseMultiplier => _jumpReleaseMultiplier;
        public float CoyoteTime => _coyoteTime;
        public float JumpBufferTime => _jumpBufferTime;
        public float BaseGravityScale => _baseGravityScale;
        public float LowJumpGravityMultiplier => _lowJumpGravityMultiplier;
        public float FallGravityMultiplier => _fallGravityMultiplier;
        public float MaxFallSpeed => _maxFallSpeed;
    }
}
