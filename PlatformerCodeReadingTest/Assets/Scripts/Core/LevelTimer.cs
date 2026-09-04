using System;
using UnityEngine;

namespace SchoolEscape.Core
{
    public sealed class LevelTimer : MonoBehaviour
    {
        public event Action<float> TimeChanged;
        public float ElapsedTime { get; private set; }
        public bool IsRunning { get; private set; }

        public void StartTimer()
        {
            ElapsedTime = 0f;
            IsRunning = true;
            TimeChanged?.Invoke(ElapsedTime);
        }

        public void StopTimer()
        {
            IsRunning = false;
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            ElapsedTime += Time.deltaTime;
            TimeChanged?.Invoke(ElapsedTime);
        }
    }
}
