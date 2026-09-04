using SchoolEscape.Core;
using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.World
{
    public sealed class RewardBrick : Brick
    {
        [SerializeField]
        private ScoreManager _scoreManager;
        [SerializeField]
        private SpriteRenderer _brickRenderer;
        [SerializeField, Min(0)]
        private int _reward = 100;
        [SerializeField]
        private Color _usedColor = new Color(0.45f, 0.42f, 0.38f);

        private bool _isUsed;

        protected override void OnHitFromBelow(PlayerMotor playerMotor)
        {
            if (_isUsed)
            {
                return;
            }

            _isUsed = true;
            _scoreManager.AddScore(_reward);
            _brickRenderer.color = _usedColor;
        }
    }
}
