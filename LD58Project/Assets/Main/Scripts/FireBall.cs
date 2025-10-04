using System.Collections;
using UnityEngine;

namespace Main.Scripts
{
    public class FireBall : MonoBehaviour
    {
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _damage = 10;
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private Transform _outline;
        
        public void Attack(Transform target)
        {
            StartCoroutine(GoToTarget(target));
        }
        
        private IEnumerator GoToTarget(Transform target)
        {
            _outline.LookAt(target.position - transform.position);
            
            while (Vector3.Distance(transform.position, target.position) > 0.1f)
            {
                var targetDirection = target.position - transform.position;
                transform.position += targetDirection.normalized * Time.deltaTime * _speed;
                yield return null;
            }

            Explode();
        }

        private void Explode()
        {
            var explosion = Instantiate(_explosionPrefab);
            explosion.transform.position = transform.position;
           
            Destroy(gameObject);
        }
    }
}