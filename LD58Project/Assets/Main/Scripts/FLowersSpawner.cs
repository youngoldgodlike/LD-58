using System.Collections;
using UnityEngine;

namespace Main.Scripts
{
    public class FLowersSpawner : MonoBehaviour
    {
        [SerializeField] private Treasure Flower_prefab;
        [SerializeField] private Vector2 _range;
         private float _spawnDuration;

        private Treasure _currentTreasure;

        private void Awake()
        {
            _spawnDuration = Random.Range(_range.x, _range.y);

            StartCoroutine(HandleSpawnDelay());
        }

        IEnumerator HandleSpawnDelay()
        {
            while (true)
            {
                if (_currentTreasure != null)
                {
                    yield return new WaitUntil(() => _currentTreasure.isTaken);
                    _spawnDuration = Random.Range(1f, 3f);
                }
                
                yield return new WaitForSeconds(_spawnDuration);

                _currentTreasure = Instantiate(Flower_prefab, transform.position, Quaternion.identity);
            }
        }
    }
}