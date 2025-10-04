using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Scripts
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private Transform _projectileSpawner;
        
        [Header("Fireballs")]
        [SerializeField] private FireBall _fireballPrefab;
        [SerializeField] private float _fireballsCooldown = 5f;
        [SerializeField] private float _fireballsCastDuration = 1f;
        [SerializeField] private int _fireballsCount = 2;

        [Header("Laser")]
        [SerializeField] private LineRenderer _laserLineRenderer;
        [SerializeField] private float _laserHitDelay = 0.5f;
        [SerializeField] private float _laserDamage = 1;
        
        [Header("Enemies")]
        [SerializeField] private Transform[] _enemies;
        
        private void Start()
        {
            _laserLineRenderer.positionCount = 2;
            
            StartCoroutine(FireBallRoutine());
            StartCoroutine(LaserRoutine());
            StartCoroutine(RotateEyeRoutine());
        }


        private IEnumerator RotateEyeRoutine()
        {
            while (true)
            {
                _projectileSpawner.Rotate(Vector3.forward * 50 * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator LaserRoutine()
        {
            var randomIndex = Random.Range(0, _enemies.Length);
            var target = _enemies[randomIndex];

            float time = 0;
            
            while (true)
            {
                _laserLineRenderer.SetPosition(0, _projectileSpawner.position);
                _laserLineRenderer.SetPosition(1, target.position);

                time += Time.deltaTime;

                if (time >= _laserHitDelay)
                {
                    Debug.Log("hit");
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
                    var randomIndex = Random.Range(0, _enemies.Length);

                    FireBall fireBall = Instantiate(_fireballPrefab);
                    fireBall.transform.position = _projectileSpawner.transform.position;
                    fireBall.Attack(_enemies[randomIndex]);
                    
                    float timeDelay = _fireballsCastDuration / _fireballsCount;
                    yield return new WaitForSeconds(timeDelay);
                }

                yield return new WaitForSeconds(_fireballsCooldown);
            }
        }
    }
}
