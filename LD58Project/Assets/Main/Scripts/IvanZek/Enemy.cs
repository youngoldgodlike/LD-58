using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour {
    [SerializeField] NavMeshAgent _agent;

    public Transform target;
    public float health;
    public float speed;
    public int power;

    void OnValidate() {
        if (_agent) TryGetComponent(out _agent);
    }

    void Awake() {
        _agent.speed = speed;
        _agent.updateRotation = false;
    }

    public void UpdateMe() {
        Profiler.BeginSample("Enemy Update");
        _agent.SetDestination(target.position);
        // transform.LookAt(target);
        transform.rotation = Quaternion.LookRotation((target.position - transform.position).WithY(0), Vector3.up);
        Profiler.EndSample();
    }
}
