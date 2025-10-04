using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class Enemy : MonoBehaviour {
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] float _radius = 0.5f;
    [SerializeField] float _height = 1f;
    [SerializeField] bool _hueto;

    public Transform target;
    public float health;
    public float speed;
    public int power;
    public int id;

    public event Action<Enemy> OnDie = delegate { };
    
    void Awake() {
        _agent.speed = speed;
        _agent.updateRotation = false;
    }

    public void Init(int id) {
        this.id = id;
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
    }

    [ContextMenu(nameof(Kill))]
    void Kill() {
        OnDie.Invoke(this);
    }
    
    void OnTriggerEnter(Collider other) {
        // todo Deal damage to player
    }
}
