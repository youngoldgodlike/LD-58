using UnityEngine;

namespace Main.Scripts
{
    public class DesktopInput
    {
        public Vector2 Move => GetMove();
        public Vector2 Look => GetLook();
        public bool Jump => GetJump();

        private readonly InputSystem_Actions _input;
        public bool IsInteract => GetInteract();

        private bool GetInteract()
        {
            if (!_isActive) return false;

            var value = _input.Player.Sprint.triggered;
            
            return  value;
        }

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