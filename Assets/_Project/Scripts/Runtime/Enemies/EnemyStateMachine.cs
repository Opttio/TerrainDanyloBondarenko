using _Project.Scripts.Core.Enums;
using UnityEngine;

namespace _Project.Scripts.Runtime.Enemies
{
    public class EnemyStateMachine : MonoBehaviour
    {
        private enum State {Patrol, ChaseTarget, GetHit, Attack}
        
        [Header("General")]
        [SerializeField] private EnemyNavMeshController _enemyNavMeshController;
        [SerializeField] private NavmeshEnemyMoveType _moveType;
        [Space] [Header("Take damage")]
        [SerializeField] private EnemyGetDamage _enemyGetDamage;

        [Space] [Header("Chase Target")]
        [SerializeField] Transform _target;
        [SerializeField] private float _distanceToChase;
        [SerializeField] private float _distanceToLoseChase;

        [Space] [Header("Attack")]
        [SerializeField] private float _distanceToAttack;
        
        private State _currentState;
        private bool _onTakeBullet = false;

        private void OnEnable() => _enemyGetDamage.OnHitComplete += OnHitAnimationFinished;
        private void OnDisable() => _enemyGetDamage.OnHitComplete -= OnHitAnimationFinished;

        private void Start()
        {
            ChangeState(State.Patrol);
        }

        private void Update()
        {
            Debug.Log(_currentState);
            ExecuteCurrentState();
        }
        
        private void ChangeState(State newState)
        {
            if (_currentState == newState)
                return;
            
            _currentState = newState;

            switch (newState)
            {
                case State.Patrol:
                    StatePatrol();
                    break;

                case State.ChaseTarget:
                    StateChaseTarget();
                    break;

                case State.GetHit:
                    StateGetHit();
                    break;

                case State.Attack:
                    StateAttack();
                    break;
            }
        }

        private void ExecuteCurrentState()
        {
            float sqrDistance = (_target.position - transform.position).sqrMagnitude;
            switch (_currentState)
            {
                case State.Patrol:
                    if (_onTakeBullet)
                    {
                        ChangeState(State.GetHit);
                    }
                    if (sqrDistance < _distanceToChase)
                    {
                        ChangeState(State.ChaseTarget);
                    }
                    break;

                case State.ChaseTarget:
                    if (_onTakeBullet)
                    {
                        ChangeState(State.GetHit);
                    }
                    if (sqrDistance  > _distanceToLoseChase)
                    {
                        ChangeState(State.Patrol);
                    }

                    if (sqrDistance < _distanceToAttack)
                    {
                        ChangeState(State.Attack);
                    }
                    break;

                case State.GetHit:
                    if (sqrDistance < _distanceToChase)
                    {
                        ChangeState(State.ChaseTarget);
                    }
                    else
                    {
                        ChangeState(State.Patrol);
                    }
                    break;

                case State.Attack:
                    if (_onTakeBullet)
                    {
                        ChangeState(State.GetHit);
                    }
                    if (sqrDistance > _distanceToAttack)
                    {
                        ChangeState(State.ChaseTarget);
                    }
                    break;
            }
        }

        private void StatePatrol()
        {
            _enemyNavMeshController.IsChasing = false;
            _enemyNavMeshController.IsAttacking = false;
            _enemyNavMeshController.AgentResume();
            _enemyNavMeshController.SetMoveTypeAgent(_moveType);
        }

        private void StateChaseTarget()
        {
            _enemyNavMeshController.IsAttacking = false;
            _enemyNavMeshController.AgentResume();
            _enemyNavMeshController.SetChaseTarget(_target);
            _enemyGetDamage.StopAttackAnim();
        }

        private void StateGetHit()
        {
            _enemyGetDamage.GetHit();
        }

        private void StateAttack()
        {
            _enemyNavMeshController.IsAttacking = true;
            _enemyNavMeshController.SetChaseTarget(_target, _distanceToAttack);
            _enemyGetDamage.PlayAttackAnim();
        }

        private void OnHitAnimationFinished()
        {
            _onTakeBullet = false;
            ChangeState(State.Patrol);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Bullet"))
                return;
            _onTakeBullet =  true;
        }
    }
}