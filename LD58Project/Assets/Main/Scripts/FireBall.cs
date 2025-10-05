using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

namespace Main.Scripts
{
    public class FireBall : MonoBehaviour {
        [SerializeField] float _startRadDelta;
        [SerializeField] float _radDeltaPerSec = 1f;
        [SerializeField] private float _speed = 10;
            
        [SerializeField] public float _damage = 10;
        [SerializeField] public float _exlposionRadius = 5;
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private Transform _outline;
        [SerializeField] private LayerMask _ememyLayer;
        
        public void Attack(Transform target)
        {
            StartCoroutine(FlyToTargetCoroutine(target));
        }
        
        IEnumerator FlyToTargetCoroutine(Transform target) {
            Vector3 targetPos = target.position;
            Vector3 maPos = transform.position;
            Vector3 flyDirection = Vector3.up;
            float currentRadDelta = _startRadDelta;
            
            while ((targetPos - maPos).sqrMagnitude > 1f) {
                Vector3 toTargetDir = targetPos - maPos;
                flyDirection = Vector3.RotateTowards(flyDirection, toTargetDir, currentRadDelta * Time.deltaTime, 0f);
                transform.position += flyDirection * (_speed * Time.deltaTime);

                yield return null;
                targetPos = target == null ? targetPos : target.position;
                maPos = transform.position;
                currentRadDelta += Time.deltaTime * _radDeltaPerSec;
            }
            
            Explode();
        }

        private void Explode()
        {
            var explosion = Instantiate(_explosionPrefab);
            explosion.transform.position = transform.position;
            explosion.transform.localScale = Vector3.one * _exlposionRadius;

            var collision = Physics.OverlapSphere(transform.position, _exlposionRadius);
            GetDamage(collision);
            
            Destroy(gameObject);
        }

        private void GetDamage(Collider[] overlap)
        {
            foreach (var collider in overlap)
            {
                if (collider.TryGetComponent(out Enemy enemy))
                    enemy.TakeDamage(_damage);
            }
        }
    }
}