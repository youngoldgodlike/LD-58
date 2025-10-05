using System.Collections;
using UnityEngine;

public class Treasure : MonoBehaviour 
{
    public int cost;
    public bool isOil;
    public bool isTaken;

    private Coroutine _disposeRoutine;
    
    [SerializeField] private float rotationAngle = 45f; // Угол поворота за один шаг
    [SerializeField] private float interval = 0.25f; 
    
    private void Start()
    {
        StartCoroutine(RotateRoutine());

        if (isOil)
            _disposeRoutine = StartCoroutine(DisposeRoutine());
    }

    private IEnumerator DisposeRoutine()
    {
        float time = 0;
        
        while (time <= 30f)
        {
            if (isTaken == true) yield break;
            
            time += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }

    private IEnumerator RotateRoutine()
    {
        while (true)
        {
            if (isTaken) yield break;
            
            yield return new WaitForSeconds(interval);
            
            transform.RotateAround(transform.position, Vector3.up, rotationAngle);
        }
    }

}
