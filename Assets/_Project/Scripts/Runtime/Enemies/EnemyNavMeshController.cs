using _Project.Scripts.Core.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Runtime.Enemies
{
    public class EnemyNavMeshController : MonoBehaviour
    {
        [SerializeField] private Transform[] _path;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _pathPointThreshold;
        [Space]
        [SerializeField] private bool _isForward;

        private int _currentTargetPoint;
        private NavmeshEnemyMoveType _moveType;
        private Transform _chaseTarget;
        private Vector3 _currentDestination;
        public bool IsChasing { get; set; }
        public bool IsAttacking { get; set; }

        public void SetMoveTypeAgent(NavmeshEnemyMoveType moveType)
        {
            _moveType = moveType;
            InitPath();
        }
        
        public void SetChaseTarget(Transform target)
        {
            IsChasing = true;
            _chaseTarget = target;
            _currentDestination = target.position;
            SetDestination(_currentDestination);
            _agent.stoppingDistance = _pathPointThreshold;
        }
        
        public void SetChaseTarget(Transform target, float stoppingDistance)
        {
            IsChasing = true;
            _chaseTarget = target;
            _currentDestination = target.position;
            SetDestination(_currentDestination);
            _agent.stoppingDistance = stoppingDistance;
        }
        
        public void AgentResume() => _agent.isStopped = false;

        private void Update()
        {
            if ((IsChasing || IsAttacking) && _chaseTarget)
            {
                float distanceToTarget = Vector3.Distance(transform.position, _chaseTarget.position);
                if (IsAttacking && distanceToTarget <= _agent.stoppingDistance + 0.05f)
                {
                    _agent.isStopped = true;
                    FaceTarget(_chaseTarget.position);
                    return;
                }
                _agent.isStopped = false;
                Vector3 destination = IsAttacking ? _chaseTarget.position - (_chaseTarget.position - transform.position).normalized * _agent.stoppingDistance : _chaseTarget.position;

                if (Vector3.Distance(_agent.destination, destination) > 0.1f)
                {
                    SetDestination(destination);
                }

                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    FaceTarget(_chaseTarget.position);
                }
            }
            else
            {
                EnemyWalking();
            }
        }

        private void InitPath()
        {
            if (_path == null || _path.Length == 0) return;
            _currentTargetPoint = _moveType switch
            {
                NavmeshEnemyMoveType.Random => Random.Range(0, _path.Length),
                NavmeshEnemyMoveType.FlipFlop => _isForward ? 0 : _path.Length - 1,
                NavmeshEnemyMoveType.Loop => _isForward ? 0 : _path.Length - 1,
                _ => 0
            };
            SetDestination(_path[_currentTargetPoint].position);
        }

        private void EnemyWalking()
        {
            if (!_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance)
            {
                _currentTargetPoint = _moveType switch
                {
                    NavmeshEnemyMoveType.Random => Random.Range(0, _path.Length),
                    NavmeshEnemyMoveType.FlipFlop => GetNextFlipFlopTarget(_currentTargetPoint),
                    NavmeshEnemyMoveType.Loop => GetNextLoopTarget(_currentTargetPoint),
                    _ => 0,
                };
                SetDestination(_path[_currentTargetPoint].position);
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

        private void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }
        private void FaceTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;
            
            if (direction.magnitude == 0)
                return;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}