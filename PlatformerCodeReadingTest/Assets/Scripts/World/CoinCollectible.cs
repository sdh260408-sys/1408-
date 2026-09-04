using SchoolEscape.Contracts;
using SchoolEscape.Core;
using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.World
{
    public sealed class CoinCollectible : MonoBehaviour, ICollectible
    {
        [SerializeField]
        private ScoreManager _scoreManager;
        [SerializeField, Min(0)]
        private int _points = 100;

        private bool _isCollected;

        public void Collect(PlayerCollector collector)
        {
            if (_isCollected)
            {
                return;
            }

            _isCollected = true;
            _scoreManager.AddScore(_points);
            Destroy(gameObject);
        }
    }
}
