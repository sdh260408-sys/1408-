using UnityEngine;

namespace SchoolEscape.Core
{
    public sealed class RespawnManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _initialSpawnPoint;

        public Vector3 CurrentSpawnPosition { get; private set; }

        private void Awake()
        {
            CurrentSpawnPosition = _initialSpawnPoint.position;
        }

        public void SetCheckpoint(Vector3 position)
        {
            CurrentSpawnPosition = position;
        }
    }
}
