using System.Collections;
using PrimeTween;
using UnityEngine;
public class Collector : MonoBehaviour {
    [SerializeField] Transform _takePoint;
    [SerializeField] float sosalTimeForOne = 0.3f;
    // MoneyStorage;
    Bagy _bagy;

    Coroutine _coroutine;

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.name, other.transform);
        _bagy = other.GetComponent<Bagy>();
        _coroutine = StartCoroutine(TakingItemsProcess());
    }
    void OnTriggerExit(Collider other) {
        StopCoroutine(_coroutine);
    }
    IEnumerator TakingItemsProcess() {
        while (_bagy.treasures.Count > 0) {
            var treasure = _bagy.Take();
            Tween.Position(treasure.transform, GetAroundPos(), _takePoint.position, sosalTimeForOne);
            Tween.LocalRotation(treasure.transform, -treasure.transform.forward, sosalTimeForOne);
            yield return Tween.Scale(treasure.transform, 0, sosalTimeForOne);

            Destroy(treasure.gameObject);
            
            yield return null;
        }
    }

    Vector3 GetAroundPos() {
        Vector3 pos = _bagy.transform.position;
        pos.x += Random.Range(-0.5f, 0.5f);
        pos.y += Random.Range(-0.5f, 0.5f);
        
        return pos;
    }
}
