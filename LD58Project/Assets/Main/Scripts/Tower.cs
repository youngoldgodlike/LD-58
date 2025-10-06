using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Scripts
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private Transform _projectileSpawner;
        [SerializeField] private Spawner _spawner;
        [SerializeField] private Player _player;
        
        [Header("Fireballs")]
        [SerializeField] private FireBall _fireballPrefab;

        [SerializeField] private float _fireballsCooldown = 5f;
        [SerializeField] private float _fireballsCastDuration = 1f;
        [SerializeField] private int _fireballsCount = 2;
        
        [Header("Laser")]
        [SerializeField] private LineRenderer _laserLineRenderer;

        [SerializeField] private float _laserHitDelay = 0.5f;
        [SerializeField] private float _laserDamage = 1;

        private Coroutine _laserRoutine;
        private Coroutine _fireBallRoutine;
        private Coroutine _delayBeforeStartAttack;

        private void Start()
        {
            // _spawner.Initialize();
            
            _laserLineRenderer.positionCount = 2;
            
            StartCoroutine(RotateEyeRoutine());
            
            StartAttack();
        }

        public void StopAttack()
        {
            if (_delayBeforeStartAttack != null)
                StopCoroutine(_delayBeforeStartAttack);
            
            if (_fireBallRoutine != null)
                StopCoroutine(_fireBallRoutine);
            
            if (_laserRoutine != null)
                StopCoroutine(_laserRoutine);
        }

        public void StartAttack()
        {
            _delayBeforeStartAttack = StartCoroutine(DelayBeforeAttackRoutine());
        }

        private IEnumerator DelayBeforeAttackRoutine()
        {
            yield return new WaitForSeconds(2.5f);
            
            _fireBallRoutine = StartCoroutine(FireBallRoutine());
            _laserRoutine = StartCoroutine(LaserRoutine());
        }

        private IEnumerator RotateEyeRoutine()
        {
            while (true)
            {
                _projectileSpawner.LookAt(_player.transform);
                yield return null;
            }
        }

        private IEnumerator LaserRoutine()
        {
            while (true)
            {
                // Проверяем, есть ли враги
                if (_spawner._spawnedEnemies.Count == 0)
                {
                    _laserLineRenderer.enabled = false;
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                
                var randomIndex = Random.Range(0, _spawner._spawnedEnemies.Count);
                var target = _spawner._spawnedEnemies[randomIndex];
                
                if (target == null)
                {
                    yield return null;
                    continue;
                }
                
                _laserLineRenderer.enabled = true;
                float time = 0;
                
                // Атакуем текущую цель, пока она жива
                while (target != null && target.health > 0)
                {
                    _laserLineRenderer.SetPosition(0, _projectileSpawner.position);
                    _laserLineRenderer.SetPosition(1, target.transform.position);

                    time += Time.deltaTime;

                    if (time >= _laserHitDelay)
                    {
                        target.TakeDamage(_laserDamage);
                        time = 0;
                    }

                    yield return null;
                }
                
                // Цель уничтожена или стала null, ищем новую
                yield return null;
            }
        }

        private IEnumerator FireBallRoutine()
        {
            while (true)
            {
                // Проверяем, есть ли враги перед атакой
                if (_spawner._spawnedEnemies.Count > 0)
                {
                    for (int i = 0; i < _fireballsCount; i++)
                    {
                        // Повторная проверка на случай, если враги закончились во время цикла
                        if (_spawner._spawnedEnemies.Count == 0)
                            break;
                            
                        var randomIndex = Random.Range(0, _spawner._spawnedEnemies.Count);
                        var target = _spawner._spawnedEnemies[randomIndex];
                        
                        if (target != null)
                        {
                            FireBall fireBall = Instantiate(_fireballPrefab);
                            fireBall.transform.position = _projectileSpawner.transform.position;
                            fireBall.Attack(target.transform);
                        }
                        
                        float timeDelay = _fireballsCastDuration / _fireballsCount;
                        yield return new WaitForSeconds(timeDelay);
                    }
                }

                yield return new WaitForSeconds(_fireballsCooldown);
            }
        }

        public void IncraseLaserDamage()
        {
            
        }

        public void IncraseLaserCount()
        {
            
        }

        public void IncraseLaserSpeed()
        {
        }

        public void IncraseFireBall_Damage()
        {
        }

        public void IncraseFireBall_Radius()
        {
        }

        public void IncraseFireBall_Count()
        {
        }
    }
}
