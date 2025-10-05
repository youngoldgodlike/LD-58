using System;
using UnityEngine;

namespace Main.Scripts
{
    public class InteractbleSystem : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        
        private IInteractable _highlightedInteractable;
        private DesktopInput _desktopInput;
        private bool _isActive = true;
        
        public void Initialize(DesktopInput input) => _desktopInput = input;
        
        public void SetActive(bool value)
        {
            _isActive = value;
            if (!_isActive)
            {
                _highlightedInteractable?.DisableOutline();
                _highlightedInteractable = null;
            }
        }
        
        private void Update()
        {
            if (!_isActive) return;
            
            CheckInteractable();

            if (_desktopInput.IsInteract && _highlightedInteractable != null)
            {
                _highlightedInteractable.Interact();
            }
        }

        public void CheckInteractable()
        {
            IInteractable hitInteractable = null;
            
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, 3f))
            {
                hit.collider.TryGetComponent(out hitInteractable);
            }
            
            if (hitInteractable != _highlightedInteractable)
            {
                _highlightedInteractable?.DisableOutline();
                _highlightedInteractable = hitInteractable;
                _highlightedInteractable?.EnableOutline();
            }
        }
    }
}