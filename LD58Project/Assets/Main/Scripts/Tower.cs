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
            _spawner.Initialize();
            
            _laserLineRenderer.positionCount = 2;
            
            StartCoroutine(RotateEyeRoutine());
        }

        private IEnumerator DelayBeforeAttackRoutine()
        {
            yield return new WaitForSeconds(2.5f);
            
            _fireBallRoutine = StartCoroutine(FireBallRoutine());
            _laserRoutine = StartCoroutine(LaserRoutine());
        }
        
        public void StartAttack()
        {
            _delayBeforeStartAttack = StartCoroutine(DelayBeforeAttackRoutine());
        }

        public void StopAttack()
        {
            if (_delayBeforeStartAttack != null)
                StopCoroutine(_delayBeforeStartAttack);
            
            StopCoroutine(_fireBallRoutine);
            StopCoroutine(_laserRoutine);
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
            var randomIndex = Random.Range(0, _spawner._spawnedEnemies.Count);
            var target = _spawner._spawnedEnemies[randomIndex];
            
            float time = 0;
            
            while (true)
            {
                if (target.health <= 0)
                {
                    StopCoroutine(_laserRoutine);
                    _laserRoutine = StartCoroutine(LaserRoutine());
                }

                if (target !=null)
                {
                    _laserLineRenderer.SetPosition(0, _projectileSpawner.position);
                    _laserLineRenderer.SetPosition(1, target.transform.position);
                }

                time += Time.deltaTime;

                if (time >= _laserHitDelay)
                {
                   target.TakeDamage(_laserDamage);
                    time = 0;
                }

                yield return null;
            }
        }

        private IEnumerator FireBallRoutine()
        {
            while (true)
            {
                for (int i = 0; i < _fireballsCount; i++)
                {
                    var randomIndex = Random.Range(0, _spawner._spawnedEnemies.Count);

                    FireBall fireBall = Instantiate(_fireballPrefab);
                    fireBall.transform.position = _projectileSpawner.transform.position;
                    fireBall.Attack(_spawner._spawnedEnemies[randomIndex].transform);
                    
                    float timeDelay = _fireballsCastDuration / _fireballsCount;
                    yield return new WaitForSeconds(timeDelay);
                }

                yield return new WaitForSeconds(_fireballsCooldown);
            }
        }
    }
}
