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
        
        private Color _basicColor;
        private Coroutine _animationRoutine;
        private static readonly int TakeDamageTrigger = Animator.StringToHash("TakeDamage");

        private void Awake()
        {
            _basicColor = _renderer.material.color;
        }

        protected override void OnCollisionEnterGetDamage(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Bullet"))
                return;

            if (_animationRoutine == null)
            {
                _animationRoutine = StartCoroutine(AnimateChangeColorRoutine(PlayDamageAnimation, ClearRoutine));
            }
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
        }

        private void PlayDamageAnimation()
        {
            _animator.SetTrigger(TakeDamageTrigger);
        }
        private void ClearRoutine()
        {
            _animationRoutine = null;
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