using System.Collections;
using UnityEngine;

namespace Main.Scripts
{
    public class FireBall : MonoBehaviour
    {
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _arcHeight = 5f;
        [SerializeField] private float _damage = 10;
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private Transform _outline;
        [SerializeField] private AnimationCurve _arcCurve;
        
        public void Attack(Transform target)
        {
            StartCoroutine(FlyToTargetCoroutine(target));
        }
        
        private IEnumerator FlyToTargetCoroutine(Transform target)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = target.position;
    
            // Рассчитываем приблизительную длину дуги
            float horizontalDistance = Vector3.Distance(
                new Vector3(startPos.x, 0, startPos.z), 
                new Vector3(targetPos.x, 0, targetPos.z)
            );
    
            // Примерная длина дуги = горизонталь + вертикальное смещение
            float arcLength = horizontalDistance + _arcHeight * 0.5f; // Упрощённая оценка
    
            // Время полёта = длина / скорость
            float flightDuration = arcLength / _speed;
    
            float elapsedTime = 0f;
    
            while (elapsedTime < flightDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / flightDuration;
        
                Vector3 currentPosXZ = Vector3.Lerp(startPos, targetPos, t);
                float baseY = Mathf.Lerp(startPos.y, targetPos.y, t);
                float arcOffset = _arcCurve.Evaluate(t) * _arcHeight;
        
                transform.position = new Vector3(currentPosXZ.x, baseY + arcOffset, currentPosXZ.z);
        
                yield return null;
            }
            // Убедиться, что дошли до финала
            transform.position = targetPos;
    
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