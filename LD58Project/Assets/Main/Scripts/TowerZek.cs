using System.Collections;
using Main.Scripts;
using UnityEngine;

public class TowerZek : MonoBehaviour
{
    [SerializeField] private Transform _projectileSpawner;

    [SerializeField] private Spawner _spawner;
        
    [Header("Fireballs")]
    [SerializeField] private FireBall _fireballPrefab;
    [SerializeField] private float _fireballsCooldown = 5f;
    [SerializeField] private float _fireballsCastDuration = 1f;
    [SerializeField] private int _fireballsCount = 2;

    [Header("Laser")]
    [SerializeField] Transform _laser;
    [SerializeField] private float _laserHitDelay = 0.5f;
    [SerializeField] private float _laserDamage = 1;
    [SerializeField] float _smoothTime = 0.1f;
    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _maxRadDelta = 1.8f;

    private Coroutine _laserRoutine;
        
    private void Start()
    {
        _spawner.Initialize();
        
        // StartCoroutine(FireBallRoutine());
        _laserRoutine = StartCoroutine(LaserRoutine());
        StartCoroutine(RotateEyeRoutine());
    }


    private IEnumerator RotateEyeRoutine()
    {
        while (true)
        {
            _projectileSpawner.Rotate(Vector3.forward * (50 * Time.deltaTime));
            yield return null;
        }
    }

    IEnumerator LaserRoutine() {
        var enemies = _spawner._spawnedEnemies;
        WaitWhile waitEnemy = new(() => enemies.Count == 0);

        Vector3 curDirection = Vector3.one;
        Vector3 curVelocity = Vector3.zero;
        Vector3 currentGroundPos = Vector3.zero;
        Vector3 targetPos;

        while (true) {
            yield return waitEnemy;

            // Водим по губам так сказать
            while (enemies.Count > 0) {
                var target = _spawner.GetRandomEnemy();
                if (!target) break;

                targetPos = target.transform.position;
                
                while (target && (targetPos - currentGroundPos).sqrMagnitude >= 0.5f) {
                    targetPos = target.transform.position;
                    curDirection = Vector3.RotateTowards(curDirection, (targetPos - currentGroundPos),
                        _maxRadDelta * Time.deltaTime, 0f);
                    // currentPos = Vector3.SmoothDamp(currentPos, targetPos, ref curVelocity, _smoothTime, _maxSpeed);
                    currentGroundPos += curDirection * (_maxSpeed * Time.deltaTime);
                    
                    Vector3 directionToNextPos = (currentGroundPos - _projectileSpawner.position).normalized;
                    
                    _laser.position = _projectileSpawner.position + directionToNextPos * _laser.localScale.y;
                    
                    Debug.DrawRay(_laser.position, directionToNextPos, Color.green, 5f);
                    Vector3 levo = Vector3.Cross(directionToNextPos, Vector3.up);
                    // Vector3 look = Vector3.Cross(directionToNextPos, levo);

                    _laser.rotation = Quaternion.LookRotation(levo, directionToNextPos);
                    
                    Debug.DrawRay(_laser.position, levo * 10f, Color.yellow, 5f);
                    // Debug.DrawRay(_laser.position, look * 10f, Color.blue, 5f);

                    yield return null;
                }
            }
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
