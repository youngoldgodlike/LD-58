using System;
using UnityEngine;

public class Lazer : MonoBehaviour {
    public float dmg = 10;
    
    void OnTriggerEnter(Collider other) {
        // if (other.gameObject.TryGetComponent(out Enemy enemy)) {
            // enemy.TakeDamage(dmg);
        // }
    }
    void OnTriggerStay(Collider other) {
        if (other.gameObject.TryGetComponent(out Enemy enemy)) {
            enemy.TakeDamage(dmg * Time.deltaTime);
        }
    }
}

