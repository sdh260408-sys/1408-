using SchoolEscape.Data;
using UnityEngine;

namespace SchoolEscape.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField]
        private PlayerInputReader _input;
        [SerializeField]
        private GroundSensor _groundSensor;
        [SerializeField]
        private MovementSettings _settings;

        private Rigidbody2D _body;
        private float _lastGroundedTime;
        private float _lastJumpPressedTime;
        private bool _shouldCutJump;

        public float VerticalVelocity => _body.velocity.y;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _lastGroundedTime = float.NegativeInfinity;
            _lastJumpPressedTime = float.NegativeInfinity;
        }

        private void Update()
        {
            if (_groundSensor.IsGrounded)
            {
                _lastGroundedTime = Time.time;
            }

            if (_input.ConsumeJumpPress())
            {
                _lastJumpPressedTime = Time.time;
            }

            if (_input.ConsumeJumpRelease())
            {
                _shouldCutJump = true;
            }
        }

        private void FixedUpdate()
        {
            ApplyHorizontalMovement();
            TryJump();
            ApplyEarlyJumpRelease();
            ApplyGravity();
            ClampFallSpeed();
        }

        private void ApplyHorizontalMovement()
        {
            float targetSpeed = _input.Horizontal * _settings.MaxSpeed;
            float rate = SelectHorizontalRate(targetSpeed);
            float horizontalVelocity = Mathf.MoveTowards(_body.velocity.x, targetSpeed, rate * Time.fixedDeltaTime);
            _body.velocity = new Vector2(horizontalVelocity, _body.velocity.y);
        }

        private float SelectHorizontalRate(float targetSpeed)
        {
            bool hasNoInput = Mathf.Abs(targetSpeed) < 0.01f;
            if (hasNoInput)
            {
                return _groundSensor.IsGrounded ? _settings.GroundDeceleration : _settings.AirDeceleration;
            }

            bool isTurningAround = Mathf.Abs(_body.velocity.x) > 0.1f
                && Mathf.Sign(targetSpeed) != Mathf.Sign(_body.velocity.x);
            if (isTurningAround)
            {
                return _settings.TurnaroundAcceleration;
            }

            return _groundSensor.IsGrounded ? _settings.GroundAcceleration : _settings.AirAcceleration;
        }

        private void TryJump()
        {
            bool canUseCoyoteTime = Time.time - _lastGroundedTime <= _settings.CoyoteTime;
            bool hasBufferedJump = Time.time - _lastJumpPressedTime <= _settings.JumpBufferTime;
            if (canUseCoyoteTime && hasBufferedJump)
            {
                _body.velocity = new Vector2(_body.velocity.x, _settings.JumpVelocity);
                _lastGroundedTime = float.NegativeInfinity;
                _lastJumpPressedTime = float.NegativeInfinity;
            }
        }

        private void ApplyEarlyJumpRelease()
        {
            if (_shouldCutJump && _body.velocity.y > 0f)
            {
                _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y * _settings.JumpReleaseMultiplier);
            }

            _shouldCutJump = false;
        }

        private void ApplyGravity()
        {
            if (_body.velocity.y < -0.01f)
            {
                _body.gravityScale = _settings.BaseGravityScale * _settings.FallGravityMultiplier;
            }
            else if (_body.velocity.y > 0.01f && !_input.JumpHeld)
            {
                _body.gravityScale = _settings.BaseGravityScale * _settings.LowJumpGravityMultiplier;
            }
            else
            {
                _body.gravityScale = _settings.BaseGravityScale;
            }
        }

        private void ClampFallSpeed()
        {
            if (_body.velocity.y < -_settings.MaxFallSpeed)
            {
                _body.velocity = new Vector2(_body.velocity.x, -_settings.MaxFallSpeed);
            }
        }

        public void Bounce(float velocity)
        {
            _body.velocity = new Vector2(_body.velocity.x, velocity);
        }

        public void Stop()
        {
            _body.velocity = Vector2.zero;
        }
    }
}
