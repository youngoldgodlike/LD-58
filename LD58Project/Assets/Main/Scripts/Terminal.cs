using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main.Configs;
using PrimeTween;
using TMPro;
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
        [SerializeField] private TowerZek _tower;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _enterClip;
        [SerializeField] private AudioClip _exitClip;
        [SerializeField] private AudioClip _updateSelectClip;
        [SerializeField] private UpdateViewConfig _config;
        [SerializeField] private List<UpdateView> _updateViews;
        [SerializeField] private Button _closebutton;
        [SerializeField] private TMP_Text _moneyText;
        private InteractbleSystem _interactbleSystem;
        private DesktopInput _input;
        private Dictionary<string, Action> _updates;
        Spawner _spawner;
        
        private int _towerLevel;
       [SerializeField] private float _initialScale = 0.01f;
        
        private bool _isOpen;
        private int _freePoints;
        private int _money;

        public string Id => "Terminal";
        public event Action OnUpgrade = delegate { };

        public void Initialize() {
            _interactbleSystem = FindObjectOfType<InteractbleSystem>();
            _tower = FindObjectOfType<TowerZek>();
            _spawner = FindObjectOfType<Spawner>();
            
            _closebutton.onClick.AddListener(Close);
            
            _updates = new Dictionary<string, Action>
            {
                {"laser_Damage", () =>
                {
                    Debug.Log("Есть контакт");
                    if (_money < 25) return;
                    _tower.IncraseLaserDamage();
                    _money -= 50;
                    OnUpgrade.Invoke();
                    UpdateMoneyUI();
                    InitializeUpdateView();
                    _audioSource.PlayOneShot(_updateSelectClip);
                    
                }},
                {"laser_Count", () =>
                    {
                        
                        Debug.Log("Есть контакт");
                        if (_money < 50) return;
                        _tower.IncraseLaserCount();
                        _money -= 50;
                        OnUpgrade.Invoke();
                        UpdateMoneyUI();
                        InitializeUpdateView();
                        _audioSource.PlayOneShot(_updateSelectClip);
                    }
                },
                {"laser_Speed", () =>
                    {
                        
                        Debug.Log("Есть контакт");
                        if (_money < 35) return;
                        _tower.IncraseLaserSpeed();
                        _money -= 50;
                        OnUpgrade.Invoke();
                        UpdateMoneyUI();
                        InitializeUpdateView();
                        _audioSource.PlayOneShot(_updateSelectClip);
                    }
                },
                {"FireBall_Damage",() =>
                    {
                        
                        Debug.Log("Есть контакт");
                        if (_money < 25) return;
                        _tower.IncraseFireBall_Damage();
                        _money -= 50;
                        OnUpgrade.Invoke();
                        UpdateMoneyUI(); 
                        InitializeUpdateView();
                        _audioSource.PlayOneShot(_updateSelectClip);
                    }
                },
                {"FireBall_Radius",() =>
                    {
                        
                        Debug.Log("Есть контакт");
                        if (_money < 30) return;                        
                        _tower.IncraseFireBall_Radius();
                        _money -= 50;
                        OnUpgrade.Invoke();
                        UpdateMoneyUI();
                        InitializeUpdateView();
                        _audioSource.PlayOneShot(_updateSelectClip);
                    }
                },
                {"FireBall_Count", () =>
                    {
                        
                        Debug.Log("Есть контакт");
                        if (_money < 50) return;
                        
                        _tower.IncraseFireBall_Count();
                        _money -= 50;
                        OnUpgrade.Invoke();
                        UpdateMoneyUI();
                        InitializeUpdateView();
                        _audioSource.PlayOneShot(_updateSelectClip);
                    }
                },
            };
            
            UpdateMoneyUI();
            InitializeUpdateView();
        }
        
        public void SetInput(DesktopInput input) {
            _input = input;
        }

        private void Update()
        {
            if (_isOpen && _input != null && _input.IsInteract)
                Close();
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

        private void InitializeUpdateView() {
            Debug.Log($"Initializng");
            // Dictionary<string, Action> available = new(_updates);
            Dictionary<string, UpdateViewData> values = new();
            List<UpdateViewData> data = _config.ViewData.ToList();
            foreach (UpdateViewData viewData in _config.ViewData) {
                values.Add(viewData.id, viewData);
            }
            
            foreach (var view in _updateViews) {
                UpdateViewData randomValue = data[Random.Range(0, values.Count)];
                values.Remove(randomValue.id);
               // var randomUpdate = _config.ViewData[randomValue.id];
               
               var value =  _updates.FirstOrDefault(x => x.Key == randomValue.id);

               view.Initialize(randomValue.Sprite, randomValue.Description, randomValue.cost, value.Value);

               for (int i = data.Count -1; i >= 0; i--) {
                   if(values.ContainsKey(data[i].id)) continue;
                   data.RemoveAt(i);
               }
            }
        }

        private void UpdateMoneyUI()
        {
            _moneyText.text = _money.ToString();
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

        public void AddMoney(int money)
        {
            _money += money;
            _moneyText.text = _money.ToString();
        }

        private IEnumerator TurnOnRoutine()
        {
            _audioSource.PlayOneShot(_enterClip);
            _player.TurnOffCameraPriority();
            Tween.Scale(_canvas.transform, _initialScale, 0.6f);
            _interactbleSystem.SetActive(false);
            _spawner.SetActive(false);
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
            
            _audioSource.PlayOneShot(_exitClip);
            Tween.Scale(_canvas.transform, 0, 0.6f);
            _player.TurnOnCameraPriority();
            _tower.StartAttack();
            _camera.Priority = -10;
            _isOpen = false;
            _interactbleSystem.SetActive(true);
            _spawner.SetActive(true);
        }
    }
}



