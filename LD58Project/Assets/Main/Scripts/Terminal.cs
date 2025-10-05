using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main.Configs;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Main.Scripts
{
    public class Terminal : MonoBehaviour, IInteractable
    {
        [SerializeField] private RectTransform _canvas;
        [SerializeField] private Outline _outline;
        [SerializeField] private Player _player;
        [SerializeField] private CinemachineCamera _camera;
        [SerializeField] private Tower _tower;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _enterClip;
        [SerializeField] private UpdateViewConfig _config;
        [SerializeField] private List<UpdateView> _updateViews;
        [SerializeField] private Button _closebutton;
        private InteractbleSystem _interactbleSystem;
        private DesktopInput _input;
        private Dictionary<string, Action> _updates;
        
        

        private int _towerLevel;
       [SerializeField] private float _initialScale = 0.01f;
        
        private bool _isOpen;
        private int _freePoints;

        public string Id => "Terminal";

        private void Awake()
        {
            _interactbleSystem = FindObjectOfType<InteractbleSystem>();
            _tower = FindObjectOfType<Tower>();
            
            _closebutton.onClick.AddListener(Close);
            
            _updates = new Dictionary<string, Action>
            {
                {"laser_Damage", _tower.IncraseLaserDamage},
                {"laser_Count", _tower.IncraseLaserCount},
                {"laser_Speed", _tower.IncraseLaserSpeed},
                {"FireBall_Damage", _tower.IncraseFireBall_Damage},
                {"FireBall_Radius", _tower.IncraseFireBall_Radius},
                {"FireBall_Count", _tower.IncraseFireBall_Count},
            };
            
            InitializeUpdateView();
        }

        private void Update()
        {
            // Обработка закрытия терминала
            if (_isOpen && _input != null && _input.IsInteract)
            {
                Close();
            }
        }

        public void EnableOutline()
        {
            _outline.enabled = true;
        }

        public void DisableOutline()
        {
            _outline.enabled = false;
        }

        public void LevelUp()
        {
            _freePoints++;
            
            InitializeUpdateView();
        }

        private void InitializeUpdateView()
        {
            foreach (var view in _updateViews)
            {
               var randomValue = Random.Range(0, _updates.Count);
               var randomUpdate = _config.ViewData[randomValue];
               
               var value =  _updates.FirstOrDefault(x => x.Key == randomUpdate.id);

               view.Initialize(randomUpdate.Sprite, randomUpdate.Description, value.Value);
            }
        }

        public void Interact()
        {
            if (!_isOpen)
            {
                Open();
            }
        }

        [ContextMenu("Open")]
        private void Open()
        {
            if (_isOpen) return;

            StartCoroutine(TurnOnRoutine());
        }


        private IEnumerator TurnOnRoutine()
        {
            _audioSource.PlayOneShot(_enterClip);
            _player.TurnOffCameraPriority();
            Tween.Scale(_canvas.transform, _initialScale, 0.6f);
            _interactbleSystem.SetActive(false);
            _camera.Priority = 10;
            _tower.StopAttack();

            yield return null;
            _isOpen = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        [ContextMenu("Close")]
        private void Close()
        {
            if (!_isOpen) return;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            Tween.Scale(_canvas.transform, 0, 0.6f);
            _player.TurnOnCameraPriority();
            _tower.StartAttack();
            _camera.Priority = -10;
            _isOpen = false;
            _interactbleSystem.SetActive(true);
        }

        public void Initialize(DesktopInput input)
        {
            _input = input;
        }
    }
}
