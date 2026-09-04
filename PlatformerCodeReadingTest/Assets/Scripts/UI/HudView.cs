using SchoolEscape.Core;
using SchoolEscape.Player;
using UnityEngine;

namespace SchoolEscape.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField]
        private ScoreManager _scoreManager;
        [SerializeField]
        private LevelTimer _levelTimer;
        [SerializeField]
        private LevelController _levelController;
        [SerializeField]
        private PlayerLife _playerLife;
        private int _score;
        private int _deaths;
        private float _elapsedTime;
        private LevelState _state;
        private GUIStyle _labelStyle;
        private GUIStyle _centeredStyle;

        private void OnEnable()
        {
            _scoreManager.ScoreChanged += ShowScore;
            _levelTimer.TimeChanged += ShowTime;
            _playerLife.DeathCountChanged += ShowDeaths;
            _levelController.StateChanged += ShowState;
        }

        private void Start()
        {
            ShowScore(_scoreManager.Score);
            ShowTime(_levelTimer.ElapsedTime);
            ShowDeaths(_playerLife.DeathCount);
            ShowState(_levelController.State);
        }

        private void OnDisable()
        {
            _scoreManager.ScoreChanged -= ShowScore;
            _levelTimer.TimeChanged -= ShowTime;
            _playerLife.DeathCountChanged -= ShowDeaths;
            _levelController.StateChanged -= ShowState;
        }

        private void ShowScore(int value)
        {
            _score = value;
        }

        private void ShowTime(float value)
        {
            _elapsedTime = value;
        }

        private void ShowDeaths(int value)
        {
            _deaths = value;
        }

        private void ShowState(LevelState value)
        {
            _state = value;
        }

        private void OnGUI()
        {
            EnsureStyles();
            GUI.Label(new Rect(24f, 18f, 320f, 45f), $"COIN SCORE  {_score}", _labelStyle);
            GUI.Label(new Rect(Screen.width - 260f, 18f, 236f, 45f), $"TIME  {_elapsedTime:0.0}", _labelStyle);
            GUI.Label(new Rect(Screen.width * 0.5f - 110f, 18f, 220f, 45f), $"FALLS  {_deaths}", _centeredStyle);
            string message = _state == LevelState.Cleared
                ? $"COURSE CLEAR!  {_elapsedTime:0.0}s\nPress R to retry"
                : "Arrow Keys / A,D: Move     Space: Jump";
            GUI.Label(new Rect(0f, Screen.height - 82f, Screen.width, 70f), message, _centeredStyle);
        }

        private void EnsureStyles()
        {
            if (_labelStyle != null)
            {
                return;
            }

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            _centeredStyle = new GUIStyle(_labelStyle)
            {
                alignment = TextAnchor.UpperCenter
            };
        }
    }
}
