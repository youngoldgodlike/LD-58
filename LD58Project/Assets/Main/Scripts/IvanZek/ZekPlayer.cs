using System;
using UnityEngine;

public class ZekPlayer : MonoBehaviour {
    [SerializeField] float speed = 5f;

    void Update() {
        if (Input.GetKey(KeyCode.A)) {
            transform.position -= transform.right * (speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position += transform.right * (speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position -= transform.forward * (speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W)) {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }
}
