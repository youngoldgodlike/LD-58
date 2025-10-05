using System;
using System.Collections;
using Main.Scripts;
using PrimeTween;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class Enemy : MonoBehaviour {
    [SerializeField] Animator _animator;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] float _radius = 0.5f;
    [SerializeField] float _height = 1f;
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] SkinnedMeshRenderer _skinned;
    [SerializeField] Material _hitMat;
    [SerializeField] GameObject _deathParticle;

    public Transform target;
    public float damage = 10;
    public float health;
    public float speed;
    public int power;

    public event Action<Enemy> OnDie = delegate { };

    Material _baseMat;
    Color _baseColor;
    Coroutine _hitRoutine;
    
    void Awake() {
        if (_skinned) _baseMat = _skinned.sharedMaterial;
        if (_meshRenderer) _baseColor = _meshRenderer.material.color;
        _agent.speed = speed;
        _agent.updateRotation = false;
    }

    public void Init() {
        if(_meshRenderer) Tween.CompleteAll(_meshRenderer.material);
        if (_skinned) Tween.CompleteAll(_skinned.material);
        _agent.isStopped = true;
        Tween.PositionY(transform, -_height, 0, 1f).OnComplete(this, enemy => {
            // _animator.SetTrigger("Walk");
            enemy._agent.isStopped = false;
        });
    }

    public void SetActive(bool value) {
        _agent.speed = value ? speed : 0;
    }
    
    public void UpdateMe() {
        Profiler.BeginSample("Enemy Update");
        Vector3 pos = transform.position;
        Vector3 rawDirectionToPlayer = (target.position - pos).normalized;

        _agent.SetDestination(target.position);
        
        transform.rotation = Quaternion.LookRotation(rawDirectionToPlayer.WithY(0), Vector3.up);
        Profiler.EndSample();
    }
    public void UpdateFixedMe() {
    }

    public void TakeDamage(float dmg) {
        health = Mathf.Clamp(health - dmg, 0, health);
        if (health == 0) OnDie.Invoke(this);

        if (_meshRenderer) {
            if(Tween.GetTweensCount(_meshRenderer.material) > 0) return;
            Tween.MaterialColor(_meshRenderer.material, Color.red, _baseColor, 0.5f);
        }
        if (_skinned) {
            // if (_hitRoutine == null) _hitRoutine = StartCoroutine(HitRoutine());
            if(Tween.GetTweensCount(_skinned.material) > 0) return;
            Tween.MaterialColor(_skinned.material, Color.red, _baseColor, 0.5f);

        }
    }

    WaitForSeconds _wait = new(0.3f);
    IEnumerator HitRoutine() {
        _skinned.material = _hitMat;
        

        yield return _wait;

        _skinned.material = _baseMat;
        _hitRoutine = null;
    }

    
    [ContextMenu(nameof(Kill))]
    void Kill() {
        // _animator.SetTrigger("Death");
        Instantiate(_deathParticle).transform.position = transform.position;
        OnDie.Invoke(this);
    }
    
    void OnTriggerEnter(Collider other) {
        // todo Deal damage to player
        other.GetComponent<Player>()?.DealDamage();
    }
}
