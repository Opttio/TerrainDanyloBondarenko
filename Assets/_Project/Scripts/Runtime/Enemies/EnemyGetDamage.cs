using System.Collections;
using _Project.Scripts.Core.ActionBases;
using UnityEngine;

namespace _Project.Scripts.Runtime.Enemies
{
    public class EnemyGetDamage : ActionBase
    {
        [SerializeField] private Color _targetColor;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _durationToColor;
        [SerializeField] private float _durationToBasicColor;
        [Space, Header("Animations")]
        [SerializeField] private Animator _animator;
        [SerializeField] private float _attackDuration;
        
        private Color _basicColor;
        private Coroutine _animationRoutine;
        private bool _isTakingDamage = false;
        private static readonly int TakeDamageTrigger = Animator.StringToHash("TakeDamage");
        private static readonly int AttackBool = Animator.StringToHash("Attack");
        public event System.Action OnHitComplete;

        private void Awake()
        {
            _basicColor = _renderer.material.color;
        }

        public void GetHit()
        {
            if (_isTakingDamage) return;
            _isTakingDamage = true;
            ClearRoutine();
            _animationRoutine = StartCoroutine(AnimateChangeColorRoutine(PlayDamageAnimation, () =>
            {
                ClearRoutine();
                _isTakingDamage = false;
                OnHitComplete?.Invoke();
            }));
        }

        public void PlayAttackAnim()
        {
            _animator.SetBool(AttackBool, true);
        }
        
        public void StopAttackAnim()
        {
            _animator.SetBool(AttackBool, false);
        }

        private IEnumerator AnimateChangeColorRoutine(System.Action onColorChangeToTarget, System.Action onChangeColorBack)
        {
            onColorChangeToTarget?.Invoke();
            
            var remainingTime = _durationToColor;
            while (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
                var lerpValue = Mathf.InverseLerp(_durationToColor, 0f, remainingTime);
                _renderer.material.color = Color.Lerp(_basicColor, _targetColor, lerpValue);
                yield return null;
            }
            _renderer.material.color = _targetColor;
            
            remainingTime = _durationToBasicColor;
            while (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
                var lerpValue = Mathf.InverseLerp(_durationToBasicColor, 0f, remainingTime);
                _renderer.material.color = Color.Lerp(_targetColor, _basicColor, lerpValue);
                yield return null;
            }
            _renderer.material.color = _basicColor;
            onChangeColorBack?.Invoke();
            OnHitComplete?.Invoke();
        }

        private void PlayDamageAnimation()
        {
            _animator.SetTrigger(TakeDamageTrigger);
        }

        private void ClearRoutine()
        {
            if (_animationRoutine != null)
            {
                StopCoroutine(_animationRoutine);
                _animationRoutine = null;
            }
        }

        protected override void OnCollisionEnterGetDamage(Collision collision)
        {
        }
        protected override void OnCollisionEnterDestroy(Collision collision)
        {
            throw new System.NotImplementedException();
        }
        protected override void OnCollisionEnterCreate(Collision collision)
        {
            throw new System.NotImplementedException();
        }
        protected override void ExecuteInternalOnStart()
        {
            throw new System.NotImplementedException();
        }
    }
}