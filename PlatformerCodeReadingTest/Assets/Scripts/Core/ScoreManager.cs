using System;
using UnityEngine;

namespace SchoolEscape.Core
{
    public sealed class ScoreManager : MonoBehaviour
    {
        public event Action<int> ScoreChanged;
        public int Score { get; private set; }

        public void ResetScore()
        {
            Score = 0;
            ScoreChanged?.Invoke(Score);
        }

        public void AddScore(int amount)
        {
            Score = Mathf.Max(0, Score + amount);
            ScoreChanged?.Invoke(Score);
        }
    }
}
