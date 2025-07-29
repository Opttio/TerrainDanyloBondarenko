using UnityEngine;

namespace _Project.Scripts.Runtime.Character.KyleTest
{
    public class KyleAnimation : MonoBehaviour
    {
        [SerializeField] private KyleController _kyleController;
        
        private Animator _animator;
        private static readonly int IsWalkingBool = Animator.StringToHash("isWalking");
        private static readonly int MoveSpeedFloat = Animator.StringToHash("MoveSpeed");
        private static readonly int JumpStartTrigger =  Animator.StringToHash("JumpStart");
        private static readonly int IsGroundedBool = Animator.StringToHash("isGrounded");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            PlayWalkAnimation();
            SetMoveSpeed();
            PlayLandAndIdleAnimation();
        }

        private void PlayWalkAnimation()
        {
            _animator.SetBool(IsWalkingBool, _kyleController.IsWalking());
        }
        
        private void SetMoveSpeed()
        {
            float speedNormalized = _kyleController.GetNormalizedSpeed();
            _animator.SetFloat(MoveSpeedFloat, speedNormalized);
        }

        private void PlayLandAndIdleAnimation()
        {
            _animator.SetBool(IsGroundedBool, _kyleController._isGrounded());
            _animator.SetBool(IsWalkingBool, _kyleController.IsWalking());
        }
        
        public void TriggerJumpAnimation()
        {
            _animator.SetTrigger(JumpStartTrigger);
        }

    }
}