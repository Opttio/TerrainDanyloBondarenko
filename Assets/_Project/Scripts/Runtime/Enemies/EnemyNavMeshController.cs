using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Runtime.Enemies
{
    public class EnemyNavMeshController : MonoBehaviour
    {
        private enum MoveType {Random, FlipFlop, Loop}

        [SerializeField] private Transform[] _path;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _pathPointThreshold;
        [Space]
        [SerializeField] private MoveType _moveType;
        [SerializeField] private bool _isForward;

        private int _currentTargetPoint;

        private void Start()
        {
            _currentTargetPoint = _moveType switch
            {
                MoveType.Random => Random.Range(0, _path.Length),
                MoveType.FlipFlop => _isForward ? 0 : _path.Length - 1,
                MoveType.Loop => _isForward ? 0 : _path.Length - 1,
                _ => 0
            };
            SetDestination();
        }

        private void Update()
        {
            if (!_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance)
            {
                _currentTargetPoint = _moveType switch
                {
                    MoveType.Random => Random.Range(0, _path.Length),
                    MoveType.FlipFlop => GetNextFlipFlopTarget(_currentTargetPoint),
                    MoveType.Loop => GetNextLoopTarget(_currentTargetPoint),
                    _ => 0,
                };
                SetDestination();
            }
        }

        private int GetNextLoopTarget(int current)
        {
            var next = GetNextIndex(current);
            if (next < 0)
                return _path.Length - 1;
            if (next >= _path.Length)
                return 0;
            return next;
        }

        private int GetNextFlipFlopTarget(int current)
        {
            var next = GetNextIndex(current);
            if (next < 0 || next >= _path.Length)
            {
                _isForward = !_isForward;
                return GetNextIndex(current);
            }
            return next;
        }

        private int GetNextIndex(int current)
        {
            return _isForward ? current + 1 : current - 1;
        }

        private void SetDestination()
        {
            _agent.SetDestination(_path[_currentTargetPoint].position);
        }
    }
}