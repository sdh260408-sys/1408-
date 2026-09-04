using UnityEngine;

namespace SchoolEscape.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool InputEnabled { get; set; } = true;

        private bool _isJumpPressed;
        private bool _isJumpReleased;

        private void Update()
        {
            if (!InputEnabled)
            {
                Horizontal = 0f;
                _isJumpPressed = false;
                JumpHeld = false;
                _isJumpReleased = false;
                return;
            }

            Horizontal = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump"))
            {
                _isJumpPressed = true;
            }

            JumpHeld = Input.GetButton("Jump");
            if (Input.GetButtonUp("Jump"))
            {
                _isJumpReleased = true;
            }
        }

        public bool ConsumeJumpPress()
        {
            if (!_isJumpPressed)
            {
                return false;
            }

            _isJumpPressed = false;
            return true;
        }

        public bool ConsumeJumpRelease()
        {
            if (!_isJumpReleased)
            {
                return false;
            }

            _isJumpReleased = false;
            return true;
        }
    }
}
