using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.5f;
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private Transform _cameraTransform;

        private CinemachineCamera _cinemachineCamera;
        private CharacterController _characterMoveController;
        private bool _isGrounded;
        private Vector2 _moveDirection;
        private float _xRotation = 0f;
        private float _verticalVelocity;
        private DesktopInput _input;

        private bool _isActive;

        private Coroutine _TurnActiveRoutine;

        private void Awake()
        {
            _characterMoveController = GetComponent<CharacterController>(); ;
            _input = new DesktopInput();
            _cinemachineCamera=  _cameraTransform.GetComponent<CinemachineCamera>();
            
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
    
        private void Update()
        {
            if (!_isActive) return;
            
            HandleGroundCheck();
            HandleJumping();
            HandleMovement();
        }

        public void TurnOffCameraPriority()
        {
            _cinemachineCamera.Priority = -10;
        }
        
        public void TurnOnCameraPriority()
        {
            _cinemachineCamera.Priority = 10;
        }
        

        private void LateUpdate()
        {
            HandleMouseLook();
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

    public class DesktopInput
    {
        public Vector2 Move => GetMove();
        public Vector2 Look => GetLook();
        public bool Jump => GetJump();

        private readonly InputSystem_Actions _input;
        private bool _isActive;

        public DesktopInput()
        {
            _input = new InputSystem_Actions();
            _input.Enable();
            Enable();
        }
        
        public void Enable() => _isActive = true;
        public void Disable() => _isActive = false;

        private Vector2 GetMove()
        {
            if (!_isActive) return Vector2.zero;
            return _input.Player.Move.ReadValue<Vector2>();
        }
        
        private Vector2 GetLook()
        {
            if (!_isActive) return Vector2.zero;
            return _input.Player.Look.ReadValue<Vector2>();
        }

        private bool GetJump()
        {
            if (!_isActive) return false;
            return _input.Player.Jump.triggered;
        }
    }
}
