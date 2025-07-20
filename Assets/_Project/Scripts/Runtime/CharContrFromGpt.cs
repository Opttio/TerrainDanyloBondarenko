using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace _Project.Scripts.Runtime
{
    public class CharContrFromGpt : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private Rigidbody _mainCharRb;
        [SerializeField] private float _playerHeight = 1.8f;
        [SerializeField] private float _stopBounceForce = 30f;

        [Header("Jump")]
        [SerializeField] private float _groundCheckDistance = 0.5f;
        [SerializeField] private float _jumpPower = 10f;

        [Header("Walk")]
        [SerializeField] private float _walkSpeed = 400f;

        [Header("Look")]
        [SerializeField] private float _sensitivity = 12f;
        [SerializeField] private Vector2 _pitchBounds = new Vector2(-80f, 80f);

        [Header("Sprint")]
        [SerializeField] private float _sprintSpeed = 720f;

        [Header("Particles")]
        [SerializeField] private ParticleSystem _landingParticles;
        [SerializeField] private ParticleSystem _sprintParticles;

        [Header("Gun")]
        [SerializeField] private Weapon _gun;

        private MyInputSystem _inputSystem;
        private Camera _characterCamera;

        private Vector2 _moveInput;
        private bool _isSprint;
        private bool _isStopBounce = true;
        private bool _wasGrounded = true;
        private float _lastFallVelocity;
        private float _cameraBounds;

        private void Awake()
        {
            _inputSystem = new MyInputSystem();

            _inputSystem.MainCharacter.Jump.performed += OnJump;
            _inputSystem.MainCharacter.Walk.performed += OnWalk;
            _inputSystem.MainCharacter.Walk.canceled += OnWalk;
            _inputSystem.MainCharacter.Look.performed += OnLook;
            _inputSystem.MainCharacter.Sprint.performed += OnSprint;
            _inputSystem.MainCharacter.Sprint.canceled += OnSprint;
            _inputSystem.MainCharacter.Shoot.performed += OnShoot;

            MoveCameraToCharacterAndLockCursor();
            MoveGunToCamera();
        }

        private void OnEnable() => _inputSystem.Enable();
        private void OnDisable() => _inputSystem.Disable();

        private void FixedUpdate()
        {
            Vector3 moveInput = new Vector3(_moveInput.x, 0f, _moveInput.y);
            Vector3 worldDirection = _characterCamera.transform.TransformDirection(moveInput);
            worldDirection.y = 0f;
            worldDirection.Normalize();

            float moveSpeed = _isSprint ? _sprintSpeed : _walkSpeed;
            Vector3 finalMove;

            bool grounded = _isGrounded(out RaycastHit hit);
            if (grounded)
            {
                Vector3 slopeNormal = hit.normal;
                Vector3 slopeDirection = Vector3.ProjectOnPlane(worldDirection, slopeNormal).normalized;
                finalMove = slopeDirection * moveSpeed * Time.fixedDeltaTime;
            }
            else
            {
                finalMove = worldDirection * moveSpeed * Time.fixedDeltaTime;
            }

            // Перешкоди спереду
            if (Physics.Raycast(_mainCharRb.position + Vector3.up * (_playerHeight * 0.4f), finalMove.normalized, 0.6f))
                finalMove = Vector3.zero;

            // Застосування швидкості
            Vector3 vel = _mainCharRb.linearVelocity;
            _mainCharRb.linearVelocity = new Vector3(finalMove.x, vel.y, finalMove.z);

            // Притискання до землі
            if (grounded && _moveInput == Vector2.zero && _isStopBounce)
                _mainCharRb.AddForce(Vector3.down * _stopBounceForce, ForceMode.Acceleration);

            PlayLandingParticles(grounded);
            _wasGrounded = grounded;

            if (_moveInput.y > 0f)
                PlaySprintParticles();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (_isGrounded(out _))
            {
                _mainCharRb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
                _isStopBounce = false;
            }
        }

        private void OnWalk(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            Vector2 delta = ctx.ReadValue<Vector2>();
            float mouseX = delta.x * _sensitivity * Time.deltaTime;
            float mouseY = delta.y * _sensitivity * Time.deltaTime;

            transform.Rotate(Vector3.up * mouseX);

            _cameraBounds -= mouseY;
            _cameraBounds = Mathf.Clamp(_cameraBounds, _pitchBounds.x, _pitchBounds.y);
            _characterCamera.transform.localRotation = Quaternion.Euler(_cameraBounds, 0f, 0f);
        }

        private void OnSprint(InputAction.CallbackContext ctx)
        {
            _isSprint = ctx.ReadValue<float>() > 0f;
        }

        private void OnShoot(InputAction.CallbackContext ctx)
        {
            _gun.Shoot();
        }

        private bool _isGrounded(out RaycastHit hit)
        {
            return Physics.SphereCast(_mainCharRb.position + Vector3.up * 0.1f, 0.3f, Vector3.down, out hit, _groundCheckDistance + 0.2f);
        }

        private void PlayLandingParticles(bool grounded)
        {
            if (!grounded)
            {
                _lastFallVelocity = -_mainCharRb.linearVelocity.y;
            }

            if (!_wasGrounded && grounded)
            {
                float fallVelocity = _lastFallVelocity;

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

        private void MoveGunToCamera()
        {
            _gun.transform.SetParent(_characterCamera.transform);
            _gun.transform.localPosition = new Vector3(0.6f, -0.6f, 1.1f);
            _gun.transform.localRotation = Quaternion.identity;
        }
    }
}