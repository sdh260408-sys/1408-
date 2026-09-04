using System;
using System.Collections;
using SchoolEscape.Player;
using SchoolEscape.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SchoolEscape.Core
{
    public sealed class LevelController : MonoBehaviour
    {
        [SerializeField]
        private PlayerLife _playerLife;
        [SerializeField]
        private RespawnManager _respawnManager;
        [SerializeField]
        private ScoreManager _scoreManager;
        [SerializeField]
        private LevelTimer _levelTimer;
        [SerializeField]
        private GoalFlag _goalFlag;
        [SerializeField, Min(0f)]
        private float _respawnDelay = 0.7f;

        public event Action<LevelState> StateChanged;
        public LevelState State { get; private set; } = LevelState.Ready;

        private void OnEnable()
        {
            _playerLife.Died += HandlePlayerDied;
            _goalFlag.Reached += CompleteLevel;
        }

        private void OnDisable()
        {
            _playerLife.Died -= HandlePlayerDied;
            _goalFlag.Reached -= CompleteLevel;
        }

        private void Start()
        {
            _scoreManager.ResetScore();
            _levelTimer.StartTimer();
            State = LevelState.Playing;
            StateChanged?.Invoke(State);
        }

        private void Update()
        {
            if (State == LevelState.Cleared && Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        private void HandlePlayerDied()
        {
            if (State == LevelState.Playing)
            {
                StartCoroutine(RespawnAfterDelay());
            }
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(_respawnDelay);
            _playerLife.RespawnAt(_respawnManager.CurrentSpawnPosition);
        }

        private void CompleteLevel()
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            State = LevelState.Cleared;
            _levelTimer.StopTimer();
            _playerLife.SetControlEnabled(false);
            StateChanged?.Invoke(State);
        }
    }
}
