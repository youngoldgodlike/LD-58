using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.5f;
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Terminal _terminal;
        [SerializeField] private GameObject _hud;
        [SerializeField] private GameObject _loseScreen;
        [SerializeField] private Image _hpFill;

        private CinemachineCamera _cinemachineCamera;
        private CharacterController _characterMoveController;
        private bool _isGrounded;
        public Vector2 _moveDirection;
        private float _xRotation = 0f;
        private float _verticalVelocity;
        private DesktopInput _input;
        private InteractbleSystem _interactbleSystem;

        public int health = 100;
        
        [SerializeField] private bool _isActive;

        private Coroutine _TurnActiveRoutine;
        public Vector3 position => transform.position;

        private void Awake()
        {
            _characterMoveController = GetComponent<CharacterController>(); ;
            _input = new DesktopInput();
            _cinemachineCamera=  _cameraTransform.GetComponent<CinemachineCamera>();
            
            _interactbleSystem = GetComponent<InteractbleSystem>();
            _interactbleSystem .Initialize(_input);
            _terminal.Initialize(_input);
            Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Enable(float delayBeforeEnable = 0)
        {
            _TurnActiveRoutine = StartCoroutine(EnableRoutine(delayBeforeEnable));
        } 

        private IEnumerator EnableRoutine(float delayBeforeEnable)
        {
            yield return new WaitForSeconds(delayBeforeEnable);
            _isActive = true;
        }

        public void Disable()
        {
            if (_TurnActiveRoutine != null)
                StopCoroutine(_TurnActiveRoutine);
            
            _isActive = false;
        }

        [ContextMenu("DealDamage")]
        public void DealDamage()
        {
            health -= 10;

            if (health < 0) health = 0;

            float value = (float) health / 100;
            _hpFill.fillAmount = value;
            
            if (health > 0) return;
            
            _loseScreen.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (!_isActive) return;
            
            HandleGroundCheck();
            HandleJumping();
            HandleMovement();
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


