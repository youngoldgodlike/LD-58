using System.Collections;
using Main.Scripts;
using PrimeTween;
using UnityEngine;
public class Collector : MonoBehaviour {
    [SerializeField] Transform _takePoint;
    [SerializeField] float sosalTimeForOne = 0.3f;
    [SerializeField] private Terminal _terminal;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _collectClip;
    
    // MoneyStorage;
    [ SerializeField] Bagy _bagy;
    [SerializeField] private TowerZek _tower;

    Coroutine _coroutine;

    void OnTriggerEnter(Collider other) {
        _coroutine = StartCoroutine(TakingItemsProcess());
    }
    void OnTriggerExit(Collider other) {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }
    IEnumerator TakingItemsProcess() {
        while (_bagy.treasures.Count > 0) {
            var treasure = _bagy.Take();
            yield return null;
            Tween.Position(treasure.transform, GetAroundPos(), _takePoint.position, sosalTimeForOne);
            Tween.LocalRotation(treasure.transform, -treasure.transform.forward, sosalTimeForOne);
            yield return Tween.Scale(treasure.transform, 0, sosalTimeForOne);
            
            if (treasure.isMeat) _tower.AddOil();
            else _terminal.AddMoney(treasure.cost);
           
            
            _audioSource.PlayOneShot(_collectClip);
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
