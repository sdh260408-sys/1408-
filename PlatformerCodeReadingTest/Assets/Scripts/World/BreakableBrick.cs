using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.World
{
    public sealed class BreakableBrick : Brick
    {
        [SerializeField]
        private SpriteRenderer _brickRenderer;
        [SerializeField, Min(0f)]
        private float _destroyDelay = 0.08f;

        private bool _isBroken;

        protected override void OnHitFromBelow(PlayerMotor playerMotor)
        {
            if (_isBroken)
            {
                return;
            }

            _isBroken = true;
            DisableCollision();
            _brickRenderer.enabled = false;
            Destroy(gameObject, _destroyDelay);
        }
    }
}
