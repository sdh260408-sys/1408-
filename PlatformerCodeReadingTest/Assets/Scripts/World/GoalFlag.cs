using System;
using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.World
{
    public sealed class GoalFlag : MonoBehaviour
    {
        public event Action Reached;
        private bool _isReached;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isReached || !other.TryGetComponent(out PlayerLife _))
            {
                return;
            }

            _isReached = true;
            Reached?.Invoke();
        }
    }
}
