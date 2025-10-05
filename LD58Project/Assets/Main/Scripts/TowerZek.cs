using System.Collections;
using System.Collections.Generic;
using Main.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TowerZek : MonoBehaviour
{
    [SerializeField] private Transform _projectileSpawner;

    [SerializeField] private Spawner _spawner;

    [Header("OIL")] public Image _oilFill;
    [SerializeField] public float _currentOil;
    public float _maxOil = 25f;
    
    [Header("Stats")]
    [SerializeField] float _laserDmg = 50;
    [SerializeField] float _laserSpeed = 9f;
    [SerializeField] float _fireballsCount = 2;
    [SerializeField] float _fireballDamage = 50f;
    [SerializeField] float _fireballRadius = 4f;
        
    [Header("Fireballs")]
    [SerializeField] private FireBall _fireballPrefab;
    [SerializeField] private float _fireballsCooldown = 5f;
    [SerializeField] private float _fireballsCastDuration = 1f;

    [Header("Laser")]
    [SerializeField] Lazer _laserPrefab;
    [SerializeField] GameObject _laserVFXPrefab;
    [SerializeField] List<Lazer> _lazers = new();
    [SerializeField] private float _laserHitDelay = 0.5f;
    [SerializeField] private float _laserDamage = 1;
    [SerializeField] float _smoothTime = 0.1f;
    [SerializeField] float _maxRadDelta = 1.8f;

    private Coroutine _laserRoutine;
    private Coroutine _oilRoutine;
    WaitWhile _waitPause;
    CustomWait _waitFireballCD;
    [SerializeField] bool _isPaused;
    
    float fireballCD() => _fireballsCooldown / _fireballsCount;
        
    private void Start() {
        _waitPause = new(() => _isPaused);
        _waitFireballCD = new(fireballCD);
        _spawner.Initialize();
        
        StartCoroutine(FireballProcess());
        var lazer = Instantiate(_laserPrefab);
        _lazers.Add(lazer);
        _laserRoutine = StartCoroutine(LaserRoutine(lazer.transform));
        StartCoroutine(RotateEyeRoutine());

        _oilRoutine = StartCoroutine(OilHandle());
    }

    private IEnumerator RotateEyeRoutine()
    {
        while (true)
        {
            _projectileSpawner.Rotate(Vector3.forward * (50 * Time.deltaTime));
            yield return null;
        }
    }

    public void AddOil()
    {
        _currentOil = Mathf.Clamp(_currentOil + 5, 0, _maxOil);

        if (_oilRoutine == null)
            _oilRoutine = StartCoroutine(OilHandle());
    }

    IEnumerator OilHandle()
    {
        while (true)
        {
            if(_currentOil <= 0)
            {
                StopAttack();
                yield return null;
                continue;
            };
            
            
            _currentOil -= Time.deltaTime;

            float value = _currentOil / _maxOil;
             _oilFill.fillAmount = value;
            yield return null;
        }
    }

    IEnumerator LaserRoutine(Transform laser) {
        Transform laservfx = Instantiate(_laserVFXPrefab).transform;
        var enemies = _spawner._spawnedEnemies;
        WaitWhile waitEnemy = new(() => enemies.Count == 0);

        Vector3 curDirection = _projectileSpawner.forward;
        Debug.DrawRay(_projectileSpawner.position, curDirection * 10f, Color.red, 2f);
        Vector3 currentGroundPos = _projectileSpawner.forward * 100f;
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
                    currentGroundPos += curDirection * (_laserSpeed * Time.deltaTime);
                    
                    Vector3 directionToNextPos = (currentGroundPos - _projectileSpawner.position).normalized;

                    laser.position = _projectileSpawner.position + directionToNextPos * laser.localScale.y;
                    laservfx.position = currentGroundPos;
                    
                    Debug.DrawRay(laser.position, directionToNextPos, Color.green, 5f);
                    Vector3 levo = Vector3.Cross(directionToNextPos, Vector3.up);
                    // Vector3 look = Vector3.Cross(directionToNextPos, levo);

                    laser.rotation = Quaternion.LookRotation(levo, directionToNextPos);
                    
                    Debug.DrawRay(laser.position, levo * 10f, Color.yellow, 5f);
                    // Debug.DrawRay(_laser.position, look * 10f, Color.blue, 5f);

                    yield return _waitPause;
                }
            }
        }
    }

    IEnumerator FireballProcess() {
        while (true) {
            yield return _waitPause;
            
            var randomIndex = Random.Range(0, _spawner._spawnedEnemies.Count);

            FireBall fireBall = Instantiate(_fireballPrefab);
            fireBall._damage = _fireballDamage;
            fireBall._exlposionRadius = _fireballRadius;
            fireBall.transform.position = _projectileSpawner.transform.position;
            fireBall.Attack(_spawner._spawnedEnemies[randomIndex].transform);

            yield return _waitFireballCD;
        }
    }
    public void IncraseLaserDamage() {
        _laserDamage += 10f;
        foreach (Lazer lazer in _lazers) {
            lazer.dmg = _laserDamage;
        }
    }
    public void IncraseLaserCount() {
        var laser = Instantiate(_laserPrefab);
        _lazers.Add(laser);
        StartCoroutine(LaserRoutine(laser.transform));
    }
    public void IncraseLaserSpeed() {
        _laserSpeed += 1f;
    }
    public void IncraseFireBall_Damage() => _fireballDamage += 10f;
    public void IncraseFireBall_Radius() => _fireballRadius += 0.5f;
    public void IncraseFireBall_Count() => _fireballsCount += 1;
    public void StopAttack() => _isPaused = true;
    public void StartAttack() => _isPaused = false;
}