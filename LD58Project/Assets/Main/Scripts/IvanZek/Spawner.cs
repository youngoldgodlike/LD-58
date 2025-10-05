using System.Collections;
using System.Collections.Generic;
using Main.Scripts;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] Player _player;
    [SerializeField] List<Enemy> _enemiesPrefab;

    [Header("Params")]
    public float requiredPower = 100;
    public float currentPower = 0;
    public float minEnemyPower = 10000;
    [SerializeField] float _checkCooldown = 2f;
    [SerializeField] Vector2 _spawnRadius = new(15, 30);
    [SerializeField] float _frontAngle = 180f;
    [SerializeField] float _farDistance = 30;
    float sqrFarDis;

    [Header("Runtime")]
    public List<Enemy> _spawnedEnemies  = new(40);

    WaitForSeconds _wait;
    WaitWhile _waitActivate;
    bool _isActive = true;
    
    [ContextMenu(nameof(Initialize))]
    public void Initialize() {
        _wait = new(_checkCooldown);
        _waitActivate = new(() => _isActive == false);
        sqrFarDis = _farDistance * _farDistance;
        
        foreach (Enemy enemy in _enemiesPrefab) {
            if (minEnemyPower > enemy.power) minEnemyPower = enemy.power;
        }
        StartCoroutine(Procces());
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse4)) {
            KillRandom();
        }
        foreach (Enemy enemy in _spawnedEnemies) {
            enemy.UpdateMe();
        }
        Vector3 playerPos = _player.position;
        foreach (Enemy enemy in _spawnedEnemies) {
            if ((enemy.transform.position - playerPos).sqrMagnitude > sqrFarDis) {
                enemy.transform.position = GetSpawnInFrontOfPlayer();
                enemy.Init();
            }
        }
    }
    void FixedUpdate() {
        foreach (Enemy enemy in _spawnedEnemies) {
            enemy.UpdateFixedMe();
        }
    }

    IEnumerator Procces() {
        while (true) {
            if (_isActive == false) yield return _waitActivate;
            
            if (currentPower >= requiredPower) {
                yield return _wait;
                continue;
            }

            List<Enemy> enemies = GetDeficientEnemies();
            if (enemies.Count == 0) {
                Debug.LogError("Doesnt have deficient enemies");
                yield return _wait;
                continue;
            }

            float pow = 0;
            foreach (Enemy en in enemies) {
                pow += en.power;
            }
            // Debug.Log(pow);
            currentPower += pow;
            
            foreach (Enemy toSpawn in enemies) {
                var enemy = Instantiate(toSpawn);
                _spawnedEnemies.Add(enemy);
                enemy.transform.position = GetSpawnPosition();
                // enemy.transform.position = GetSpawnInFrontOfPlayer();
                enemy.Init();
                enemy.OnDie += KillEnemy;
                enemy.target = _player.transform;

                yield return null;
            }
            
            yield return _wait;
        }
    }
    List<Enemy> GetDeficientEnemies() {
        float sum = requiredPower - currentPower + minEnemyPower;
        List<Enemy> enemies = new(8);
        List<Enemy> availableEnemies = new(_enemiesPrefab.Count);
        foreach (Enemy prefab in _enemiesPrefab) {
            if (prefab.power < sum) availableEnemies.Add(prefab);
        }

        int maxIters = 100, iters = 0;
        bool keepGoing = true;
        // Debug.Log(availableEnemies.Count);
        while (keepGoing && iters < maxIters) {
            var enemy = availableEnemies[Random.Range(0, availableEnemies.Count)];
            enemies.Add(enemy);
            sum -= enemy.power;

            // update list
            for (int i = availableEnemies.Count - 1; i >= 0; i--) {
                // Debug.Log(i);
                if (availableEnemies[i].power > sum) 
                    availableEnemies.RemoveAt(i);
            }

            if (availableEnemies.Count == 0 || sum <= 0) keepGoing = false;

            iters++;
        }

        if (iters >= maxIters) Debug.LogError("EROEROWEORWEOROE");

        return enemies;
    }

    RaycastHit[] hits = new RaycastHit[1];
    Vector3 GetSpawnPosition() {
        float angle = Random.Range(0f, 360f);
        Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
        Vector3 targetSpawn = (_player.position + direction * Random.Range(_spawnRadius.x, _spawnRadius.y));
        // Debug.DrawRay(targetSpawn, Vector3.up, Color.red,10f);
        if (Physics.RaycastNonAlloc(targetSpawn.WithY(30), Vector3.down, hits, 50f, LayerMask.GetMask(new []{"Field"})) > 0) {
            return hits[0].point;
        } else {
            Debug.LogError($"No ground founded??");
            return targetSpawn;
        }
    }
    Vector3 GetSpawnInFrontOfPlayer() {
        float angle = Random.Range(0f, _frontAngle);
        
        Vector3 moveDir = _player._moveDirection == Vector2.zero ? _player.transform.forward 
            : _player.transform.TransformDirection(new(_player._moveDirection.x, 0, _player._moveDirection.y));
        Debug.DrawRay(_player.position, moveDir * 5f, Color.blue, 1f);
        
        Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * (Quaternion.AngleAxis(-90f, Vector3.up) * moveDir);
        Vector3 targetSpawn = (_player.position + direction * Random.Range(_spawnRadius.x, _spawnRadius.y));
        Debug.DrawRay(targetSpawn, Vector3.up, Color.red,10f);
        
        if (Physics.RaycastNonAlloc(targetSpawn.WithY(3), Vector3.down, hits) > 0) {
            Debug.DrawRay(hits[0].point, Vector3.up * 5f, Color.green, 1f);
            return hits[0].point;
        } else {
            Debug.LogError($"No ground founded??");
            return targetSpawn;
        }
    }

    public void SetActive(bool value) {
        _isActive = value;
        foreach (Enemy e in _spawnedEnemies) {
            e.SetActive(value);
        }
    }
    [ContextMenu(nameof(KillAll))]
    public void KillAll() {
        foreach (Enemy e in _spawnedEnemies) {
            Destroy(e.gameObject);
        }
        _spawnedEnemies.Clear();
        currentPower = 0;
    }
    [ContextMenu(nameof(KillRandom))]
    public void KillRandom() {
        if(_spawnedEnemies.Count == 0) return;
        int id = Random.Range(0, _spawnedEnemies.Count);
        KillEnemy(_spawnedEnemies[id]);
    }
    void KillEnemy(Enemy enemy) {
        currentPower -= enemy.power;
        _spawnedEnemies.Remove(enemy);
        Tween.StopAll(enemy.transform);
        Destroy(enemy.gameObject);
    }
    public Enemy GetRandomEnemy() {
        return _spawnedEnemies[Random.Range(0, _spawnedEnemies.Count)];
    }
    public Enemy GetClosestToPlayer() {
        return null;
    }
}
