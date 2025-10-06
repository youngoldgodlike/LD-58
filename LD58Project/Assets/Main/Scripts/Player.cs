using System;
using System.Collections;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Main.Scripts
{
    public class Player : MonoBehaviour
    {
        private static readonly int IsRun = Animator.StringToHash("isRun");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        private static readonly int IsFuck = Animator.StringToHash("isFuck");

        [Header("Parameters")]
        [SerializeField] private float _moveSpeed = 1.5f;
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _healthRegen = 60f;
        
        [Header("Dependencies")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Terminal _terminal;
        [SerializeField] private GameObject _hud;
        [SerializeField] private GameObject _loseScreen;
        [SerializeField] private Image _hpFill;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _hitClip;
        [SerializeField] private AudioClip[] _steps;
        public AudioClip _portalClip;

        [SerializeField] private Animator _animator;
        private CinemachineCamera _cinemachineCamera;
        private CharacterController _characterMoveController;
        private bool _isGrounded;
        public Vector2 _moveDirection;
        private float _xRotation = 0f;
        private float _verticalVelocity;
        private DesktopInput _input;
        private InteractbleSystem _interactbleSystem;

        public int maxHealth = 100;
        public float health = 100;

        [SerializeField] private bool _isActive;

        private Coroutine _TurnActiveRoutine;
        private Coroutine _currentFuckRoutine;

        Tween _takeDmg;
        float _healthDelay = 0f;
        private bool _isFuck;
        private float _stepTimer;
        private float _stepInterval = 0.5f;

        public Vector3 position => transform.position;


        private void PlayFootstepSound()
        {
            if (_steps.Length == 0) return;
            
            _audioSource.volume = 0.15f;
            _audioSource.PlayOneShot(_steps[Random.Range(0, _steps.Length)]);
        }

        public void Initialize() {
            _characterMoveController = GetComponent<CharacterController>(); ;
            _input = new DesktopInput();
            _cinemachineCamera = _cameraTransform.GetComponent<CinemachineCamera>();
            StartCoroutine(HealthRegeneration());
            
            _interactbleSystem = GetComponent<InteractbleSystem>();
            _interactbleSystem .Initialize(_input);
            _terminal?.SetInput(_input);
            // Enable();
            
            // Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (!_isActive) return;

            if (!_isFuck)
            {
                _animator.SetBool(IsGrounded, _isGrounded);
                _animator.SetBool(IsRun,  _moveDirection != Vector2.zero);
            }

            if (_input.IsFuck && !_isFuck)
            {
                if (_currentFuckRoutine != null)
                    StopCoroutine(_currentFuckRoutine);

                _currentFuckRoutine =  StartCoroutine(FuckRoutine());
                _animator.SetBool(IsFuck, true);
            }
            
            HandleGroundCheck();
            HandleJumping();
            HandleMovement();
            HandleFootsteps();
            
        }

        public void TeleportTo(Transform target)
        {

            _audioSource.volume = 0.35f;
            _audioSource.PlayOneShot(_portalClip);
            _characterMoveController.enabled = false;

            transform.position = target.position;
            transform.rotation = target.rotation;

            _characterMoveController.enabled = true;
        }

        public void Enable(float delayBeforeEnable = 0) {
            Cursor.lockState = CursorLockMode.Locked;
            _TurnActiveRoutine = StartCoroutine(EnableRoutine(delayBeforeEnable));
        }

        private IEnumerator HealthRegeneration() {
            while (true) {
                if (_healthDelay > 0) {
                    _healthDelay -= Time.deltaTime;
                    yield return null;
                    continue;
                }

                if (health == maxHealth) {
                    yield return null;
                    continue;
                }
                health = Mathf.Clamp(health + Time.deltaTime, 0, maxHealth);
                yield return null;
            }
        }

        private IEnumerator EnableRoutine(float delayBeforeEnable)
        {
            yield return new WaitForSeconds(delayBeforeEnable);
            _isActive = true;
        }

        public void Disable() {
            Cursor.lockState = CursorLockMode.None;
            if (_TurnActiveRoutine != null)
                StopCoroutine(_TurnActiveRoutine);
            
            _isActive = false;
        }

        [ContextMenu("DealDamage")]
        public void DealDamage()
        {
            if (_takeDmg.isAlive)
                return;

            _takeDmg = Tween.Custom(0, 1, 1f, x => { });
            health -= 10;
            _healthDelay = 5f;

            if (health < 0) health = 0;

            float value = (float) health / 100;
            _hpFill.fillAmount = value;
            
            _audioSource.volume = 0.70f;
            _audioSource.PlayOneShot(_hitClip);
            
            if (health > 0) return;
            
            _loseScreen.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private IEnumerator FuckRoutine()
        {
            _isFuck = true;

            yield return new WaitForSeconds(5f);
            
            
            _isFuck = false;
            _animator.SetBool(IsFuck, false);
        }

        private void LateUpdate()
        {
            if (!_isActive) return;       
            HandleMouseLook();
        }

        public void TurnOnCameraPriority()
        {
            _hud.SetActive(true);
            _cinemachineCamera.Priority = 10;
            Enable(0.7f);
        }

        public void TurnOffCameraPriority()
        {
            _cinemachineCamera.Priority = -10;
            _hud.SetActive(false);
            Disable();
        }

        public void TryAgain()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void HandleGroundCheck()
        {
            // Более надёжная проверка земли через встроенный метод + небольшое прижатие
            _isGrounded = _characterMoveController.isGrounded;
        }
    
        private void HandleJumping()
        {
            if (_isGrounded && _input.Jump)
            {
                // Формула для расчёта начальной скорости прыжка: v = sqrt(2 * h * g)
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * 2f * Mathf.Abs(_gravity));
            }
        }
    
        private void HandleMovement()
        {
            _moveDirection = _input.Move;
            Vector3 move = transform.TransformDirection(
                new Vector3(_moveDirection.x, 0, _moveDirection.y)) * _moveSpeed;
                
            // Определяем, движется ли персонаж
            
            // Применяем гравитацию
            if (_isGrounded && _verticalVelocity < 0)
            {
                // Небольшое прижатие к земле для стабильности isGrounded
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }
            
            move.y = _verticalVelocity;
            
            _characterMoveController.Move(move * Time.deltaTime);
        }

        private void HandleFootsteps()
        {
            // Проверяем: персонаж на земле И движется
            bool isMoving = _moveDirection.magnitude > 0.1f;
            
            if (_isGrounded && isMoving && !_isFuck)
            {
                _stepTimer += Time.deltaTime;
                
                if (_stepTimer >= _stepInterval)
                {
                    PlayFootstepSound();
                    _stepTimer = 0f;
                }
            }
            else
            {
                // Сбрасываем таймер, если не идём
                _stepTimer = 0f;
            }
        }

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = _input.Look * _mouseSensitivity;

            _xRotation -= mouseDelta.y;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseDelta.x);
        }
    }

    
}


