
using _Project.Scripts.Core.Controllers;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Runtime.Character.KyleTest
{
    public class KyleController : MonoBehaviour, MyInputSystem.IMainCharacterActions
    {
        [Header("General")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _cameraPosition;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private float _cameraRotationSpeed = 5f;
        [SerializeField] private float _cameraFollowSpeed = 20f;
        [Header("Walk")]
        [SerializeField] private float _walkSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 90f;
        [SerializeField] private float _sprintSpeed = 25f;
        [SerializeField] private float _setSprintSpeedTime = 4f;
        [SerializeField] private float _setWalkSpeedTime = 2f;
        [Header("Jump")]
        [SerializeField] private float _groundCheckDistance = 1f;
        [SerializeField] private float _jumpPower = 10f;
        [Header("Animation")]
        [SerializeField] private KyleAnimation _kyleAnimation;

        
        private Camera _characterCamera;
        private Vector2 _moveInput;
        private bool _isWalking;
        private bool _isSprint;
        private bool _sprintPressed;
        private Coroutine _accelerateRoutine;
        private Coroutine _decelerateRoutine;
        private float _currentSpeed;

        private void Awake()
        {
            MoveCameraToCameraPoint();
            _currentSpeed = _walkSpeed;
            
            InputController.SubscribeOnInput(this);
        }

        private void OnEnable() => InputController.EnableInputSystem();

        private void OnDisable() => InputController.DisableInputSystem();

        private void OnDestroy() => InputController.UnsubscribeOnInput(this);

        private void FixedUpdate()
        {
            Move();
            AccelerateDecelerateSwitcher();
            // Debug.Log($"[Player] Pos: {transform.position}, Velocity: {_rigidbody.linearVelocity}");
        }

        private void LateUpdate()
        {
            SetLookAtCameraLerp();
        }

        private void AccelerateDecelerateSwitcher()
        {
            _isSprint = _sprintPressed && _moveInput.y > 0;

            if (_isSprint && _accelerateRoutine == null && _currentSpeed != _sprintSpeed)
            {
                if (_decelerateRoutine != null)
                {
                    StopCoroutine(_decelerateRoutine);
                    _decelerateRoutine = null;
                }
                _accelerateRoutine = StartCoroutine(SetSprintSpeed());
            }
            else if (!_isSprint && _decelerateRoutine == null && _currentSpeed != _walkSpeed)
            {
                if (_accelerateRoutine != null)
                {
                    StopCoroutine(_accelerateRoutine);
                    _accelerateRoutine = null;
                }
                _decelerateRoutine = StartCoroutine(SetWalkSpeed());
            }
        }

        private void Move()
        {
            var rawDirection = new Vector3(0, 0, _moveInput.y);
            var characterDirection = transform.TransformDirection(rawDirection).normalized;
            Vector3 targetVelocity = characterDirection * _currentSpeed;
            targetVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = targetVelocity;
            
            float rotationAmount = _moveInput.x * (_rotationSpeed * Time.fixedDeltaTime);
            Quaternion deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
            _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
        }

        private IEnumerator SetSprintSpeed()
        {
            float elapsed = 0f;
            while (elapsed < _setSprintSpeedTime)
            {
                elapsed += Time.fixedDeltaTime;
                float lerpValue = elapsed / _setSprintSpeedTime;
                _currentSpeed = Mathf.Lerp(_walkSpeed, _sprintSpeed, lerpValue);
                yield return null;
            }
            _currentSpeed = _sprintSpeed;
            _accelerateRoutine = null;
        }

        private IEnumerator SetWalkSpeed()
        {
            var newCurrentSpeed = _currentSpeed;
            float elapsed = 0f;
            while (elapsed < _setWalkSpeedTime)
            {
                elapsed += Time.fixedDeltaTime;
                float lerpValue = elapsed / _setWalkSpeedTime;
                _currentSpeed = Mathf.Lerp(newCurrentSpeed, _walkSpeed, lerpValue);
                yield return null;
            }
            _currentSpeed = _walkSpeed;
            _decelerateRoutine = null;
        }

        public bool IsWalking()
        {
            _isWalking = _moveInput != Vector2.zero;
            return _isWalking;
        }

        public float GetNormalizedSpeed()
        {
            return Mathf.InverseLerp(_walkSpeed, _sprintSpeed, _rigidbody.linearVelocity.magnitude);
        }
        
        public bool _isGrounded()
        {
            Vector3 gap = new Vector3(0, 0.5f, 0);
            if (Physics.Raycast(_rigidbody.transform.position + gap, Vector3.down, _groundCheckDistance))
                return true;
            else
                return false;
        }

        private void MoveCameraToCameraPoint()
        {
            _characterCamera = Camera.main;
            if (_characterCamera)
            {
                _characterCamera.transform.SetParent(null);
                _characterCamera.transform.position = _cameraPosition.position;
                _characterCamera.transform.LookAt(_cameraTarget);
            }
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void SetLookAtCameraLerp()
        {
            if (!_characterCamera || !_cameraTarget || !_cameraPosition) return;
            _characterCamera.transform.position = Vector3.Lerp(_characterCamera.transform.position, _cameraPosition.position,Time.deltaTime * _cameraFollowSpeed);
            
            Vector3 direction = _cameraTarget.position - _characterCamera.transform.position;
            if (direction.sqrMagnitude < 0.001f) return;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _characterCamera.transform.rotation = Quaternion.Slerp(_characterCamera.transform.rotation, targetRotation, Time.deltaTime * _cameraRotationSpeed);
        }

        public void OnWalk(InputAction.CallbackContext walkValue)
        {
            _moveInput = walkValue.ReadValue<Vector2>();
            IsWalking();
        }

        public void OnSprint(InputAction.CallbackContext sprintButton)
        {
            if (sprintButton.phase != InputActionPhase.Performed) return;
            _sprintPressed = sprintButton.ReadValue<float>() > 0f;
        }

        public void OnJump(InputAction.CallbackContext jumpButton)
        {
            if (jumpButton.phase != InputActionPhase.Performed) return;
            if (_isGrounded())
            {
                _rigidbody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
                _kyleAnimation.TriggerJumpAnimation();
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
        }

        public void OnCollect(InputAction.CallbackContext context)
        {
        }
    }
}