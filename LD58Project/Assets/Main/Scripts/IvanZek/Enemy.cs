using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class Enemy : MonoBehaviour {
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] float _radius = 0.5f;
    [SerializeField] float _height = 1f;
    [SerializeField] bool _hueto;
    [SerializeField] MeshRenderer _meshRenderer;

    public Transform target;
    public float health;
    public float speed;
    public int power;

    public event Action<Enemy> OnDie = delegate { };

    Color _baseColor;
    
    void Awake() {
        _baseColor = _meshRenderer.material.color;
        _agent.speed = speed;
        _agent.updateRotation = false;
    }

    public void Init() {
        _agent.isStopped = true;
        Tween.PositionY(transform, -_height, 0, 1f).OnComplete(this, enemy => enemy._agent.isStopped = false);
    }

    public void SetActive(bool value) {
        _agent.isStopped = value;
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

        if(Tween.GetTweensCount(_meshRenderer.material) > 0) return;
        Tween.MaterialColor(_meshRenderer.material, Color.red, _baseColor, 0.5f);
    }

    [ContextMenu(nameof(Kill))]
    void Kill() {
        OnDie.Invoke(this);
    }
    
    
    void OnTriggerEnter(Collider other) {
        // todo Deal damage to player
    }
}
