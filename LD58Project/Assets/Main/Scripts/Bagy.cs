using System;
using System.Collections.Generic;
using UnityEngine;

public class Bagy : MonoBehaviour {
    [SerializeField] Transform _container;
    public List<Treasure> treasures = new();

    void Awake() {
        foreach (Treasure treasure in treasures) {
            treasure.transform.position = transform.position;
            treasure.transform.parent = transform;
        }
    }

    public void Put(Treasure treasure) {
        treasure.transform.parent = _container;
        treasure.gameObject.SetActive(false);
    }
    public Treasure Take() {
        var treasure = treasures[0];
        treasures.RemoveAt(0);
        return treasure;
    }
}
