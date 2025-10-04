using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] Transform _player;
    [SerializeField] List<Enemy> _enemiesPrefab;

    [Header("Params")]
    public float requiredPower = 100;
    public float currentPower = 0;
    public float minEnemyPower = 10000;
    [SerializeField] float _checkCooldown = 2f;
    [SerializeField] Vector2 _spawnRadius = new(5, 10);
    [SerializeField] float _spawnY = 0;
    WaitForSeconds _wait;

    [Header("Runtime")]
    public List<Enemy> _spawnedEnemies = new(40);
    
    void Awake() {
        _wait = new(_checkCooldown);
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
    }
    void FixedUpdate() {
        foreach (Enemy enemy in _spawnedEnemies) {
            enemy.UpdateFixedMe();
        }
    }

    IEnumerator Procces() {
        while (true) {
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
                enemy.Init(_spawnedEnemies.Count - 1);
                enemy.OnDie += KillEnemy;
                enemy.target = _player;
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

        int maxIters = 30, iters = 0;
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
        Debug.DrawRay(targetSpawn, Vector3.up, Color.red,10f);
        if (Physics.RaycastNonAlloc(targetSpawn.WithY(30), Vector3.down, hits) > 0) {
            return hits[0].point;
        } else {
            Debug.LogError($"No ground founded??");
            return targetSpawn;
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
        int id = Random.Range(0, _spawnedEnemies.Count);
        currentPower -= _spawnedEnemies[id].power;
        Destroy(_spawnedEnemies[id].gameObject);
        _spawnedEnemies.RemoveAt(id);
    }
    void KillEnemy(Enemy enemy) {
        currentPower -= enemy.power;
        _spawnedEnemies.RemoveAt(enemy.id);
        Destroy(enemy.gameObject);
    }
}
