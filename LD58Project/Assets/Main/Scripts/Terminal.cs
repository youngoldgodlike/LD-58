using System;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;


namespace Main.Scripts
{
    public class Terminal : MonoBehaviour
    {
        [SerializeField] private RectTransform _canvas;
        [SerializeField] private Outline _outline;
        [SerializeField] private Player _player;
        [SerializeField] private CinemachineCamera _camera;
        [SerializeField] private Tower _tower;
        private bool _isOpen;
        
        public void EnableOutline()
        {
            _outline.enabled = true;
        }

        public void DisableOutline()
        {
            _outline.enabled = false;
        }

        public void TryOpen()
        {
            if (_isOpen) Close();
            else Open();
        }

        [ContextMenu("Open")]
        private void Open()
        {
            if (_isOpen ) return;
            
            Tween.Scale(_canvas.transform, 1, 0.6f);
            _player.TurnOffCameraPriority();
            _camera.Priority = 10;
            _tower.StopAttack();
            _isOpen = true;
        }

        [ContextMenu("Close")]

        private void Close()
        {
            if (!_isOpen) return;
            
            Tween.Scale(_canvas.transform, 0, 0.6f);
            _player.TurnOnCameraPriority();
            _tower.StartAttack();
            _camera.Priority = -10;
            _isOpen = false;
        }
    }
}
