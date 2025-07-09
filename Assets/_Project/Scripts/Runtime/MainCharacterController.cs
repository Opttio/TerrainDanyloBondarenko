using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace _Project.Scripts.Runtime
{
    public class MainCharacterController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private Rigidbody _mainCharRb;
        [SerializeField] private float _playerHeight = 1.6f;
        [SerializeField] private float _stopBounceForce = 20f;
        [Header("Jump")]
        [SerializeField] private float _groundCheckDistance = 1f;
        [SerializeField] private float _jumpPower = 10f;
        [Header("Walk")]
        [SerializeField] private float _walkSpeed = 30f;
        [Header("look")]
        [SerializeField] private float _sensitivity = 20f;
        [SerializeField] private Vector2 _pitchBounds = new Vector2(-80f, 80f);
        [Header("Sprint")]
        [SerializeField] private float _sprintSpeed = 700f;
        [Header ("Particles")]
        [SerializeField] private ParticleSystem _landingParticles;
        [SerializeField] private ParticleSystem _sprintParticles;
        
        private MyInputSystem _inputSystem;
        
        private Camera _characterCamera;
        private float _cameraBounds;
        private Vector2 _moveInput;
        private bool _isSprint;
        private bool _isStopBounce = true;
        private bool _wasGrounded = true;
        private float _lastFallVelocity;
        
        private void Awake()
        {
            #region EvetSubscribes
            _inputSystem = new MyInputSystem();
            _inputSystem.MainCharacter.Jump.performed += OnJump;
            _inputSystem.MainCharacter.Walk.performed += OnWalk;
            _inputSystem.MainCharacter.Walk.canceled += OnWalk;
            _inputSystem.MainCharacter.Look.performed += OnLook;
            _inputSystem.MainCharacter.Sprint.performed += OnSprint;
            _inputSystem.MainCharacter.Sprint.canceled += OnSprint;
            #endregion
            
            MoveCameraToCharacterAndLockCursor();
        }

        private void FixedUpdate()
        {
            // Debug.Log(_isStopBounce);
            var rawDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
            var characterDirection = _characterCamera.transform.TransformDirection(rawDirection).normalized;
            var moveSpeed = _isSprint ? _sprintSpeed : _walkSpeed;
            var move = characterDirection * (moveSpeed * Time.fixedDeltaTime);
            _mainCharRb.linearVelocity = new Vector3(move.x, _mainCharRb.linearVelocity.y, move.z);
            
            if (_isGrounded() && _moveInput == Vector2.zero && _isStopBounce)
            {
                _mainCharRb.AddForce(Vector3.down * _stopBounceForce, ForceMode.Acceleration);
                _isStopBounce = true;
            }

            PlayLandingParticles();
            _wasGrounded = _isGrounded();
            
            if (_moveInput.y > 0f)
                PlaySprintParticles();
        }

        private void OnEnable() => _inputSystem.Enable();

        private void OnDisable() => _inputSystem.Disable();

        private void OnJump(InputAction.CallbackContext jumpButton)
        {
            if (_isGrounded())
            {
                _mainCharRb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
                _isStopBounce = false;
            }
        }

        private void OnWalk(InputAction.CallbackContext walkValue)
        {
            _moveInput = walkValue.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext deltaValue)
        {
            var delta = deltaValue.ReadValue<Vector2>();
            var mouseX = delta.x * _sensitivity * Time.deltaTime;
            var mouseY = delta.y * _sensitivity * Time.deltaTime;
            
            transform.Rotate(Vector3.up * mouseX);

            _cameraBounds -= mouseY;
            _cameraBounds = Mathf.Clamp(_cameraBounds, _pitchBounds.x, _pitchBounds.y);
            _characterCamera.transform.localRotation = Quaternion.Euler(_cameraBounds, 0f, 0f);
        }

        private void OnSprint(InputAction.CallbackContext sprintButton)
        {
            _isSprint = sprintButton.ReadValue<float>() > 0f;
        }

        private bool _isGrounded()
        {
            if (Physics.Raycast(_mainCharRb.transform.position, Vector3.down, _groundCheckDistance))
            {
                StartCoroutine(SetStopBounceAfterDelay(1f));
                return true;
            }
            else 
                return false;
        }

        private IEnumerator SetStopBounceAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _isStopBounce = true;
        }

        private void PlayLandingParticles()
        {
                if (!_isGrounded())
                    _lastFallVelocity = -_mainCharRb.linearVelocity.y;

                if (!_wasGrounded && _isGrounded())
                {
                    var fallVelocity = _lastFallVelocity;
                    
                    if (fallVelocity >= 3.5f && _landingParticles != null)
                    {
                        float t = Mathf.InverseLerp(3.5f, 20f, fallVelocity);

                        var emission = _landingParticles.emission;
                        var burst = emission.GetBurst(0);
  
                        burst.count = Mathf.Lerp(25f, 100f, t);
                        emission.SetBurst(0, burst);

                        var main = _landingParticles.main;
                        main.startSize = new ParticleSystem.MinMaxCurve(
                            Mathf.Lerp(0.02f, 0.15f, t),
                            Mathf.Lerp(0.02f, 0.25f, t)
                        );

                        main.startSpeed = Mathf.Lerp(3.5f, 8f, t);

                        _landingParticles.Play();
                    }
                }
        }

        private void PlaySprintParticles()
        {
            if (_isSprint && !_sprintParticles.isPlaying)
                _sprintParticles.Play();
            if (!_isSprint && _sprintParticles.isPlaying)
                _sprintParticles.Stop();
        }

        private void MoveCameraToCharacterAndLockCursor()
        {
            _characterCamera = Camera.main;
            if (_characterCamera != null)
            {
                _characterCamera.transform.SetParent(transform);
                _characterCamera.transform.localPosition = new Vector3(0, _playerHeight, 0);
                _characterCamera.transform.localRotation = Quaternion.identity;
            }
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}