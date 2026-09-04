using System;
using SchoolEscape.Contracts;
using UnityEngine;

namespace SchoolEscape.Player
{
    public sealed class PlayerLife : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private PlayerInputReader _input;
        [SerializeField]
        private PlayerMotor _motor;
        [SerializeField]
        private Collider2D _bodyCollider;
        [SerializeField]
        private SpriteRenderer _bodyRenderer;

        public event Action Died;
        public event Action<int> DeathCountChanged;
        public int DeathCount { get; private set; }
        public bool IsAlive { get; private set; } = true;

        public void TakeDamage(int amount)
        {
            if (!IsAlive || amount <= 0)
            {
                return;
            }

            IsAlive = false;
            DeathCount++;
            DeathCountChanged?.Invoke(DeathCount);
            SetVisibleAndSolid(false);
            SetControlEnabled(false);
            _motor.Stop();
            Died?.Invoke();
        }

        public void RespawnAt(Vector3 position)
        {
            transform.position = position;
            IsAlive = true;
            SetVisibleAndSolid(true);
            SetControlEnabled(true);
        }

        public void SetControlEnabled(bool enabled)
        {
            _input.InputEnabled = enabled;
            if (!enabled)
            {
                _motor.Stop();
            }
        }

        private void SetVisibleAndSolid(bool enabled)
        {
            _bodyCollider.enabled = enabled;
            _bodyRenderer.enabled = enabled;
        }
    }
}
