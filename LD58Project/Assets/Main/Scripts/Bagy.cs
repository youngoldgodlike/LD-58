using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bagy : MonoBehaviour {
    [SerializeField] Transform _container;
    public List<Treasure> treasures = new();

    [SerializeField] private int _capacity = 10;
    [SerializeField] private Image _progressBarFill;

    private void Awake()
    {
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Treasure treasure))
        {
            if (treasure.isTaken) return;
            
            Put(treasure);
        }
    }

    private void UpdateUI()
    {
        var value = (float)treasures.Count / _capacity;
        _progressBarFill.fillAmount = value;
    }

    private void Put(Treasure treasure)
    {
        if(treasures.Count >= _capacity) return;
        
        treasure.transform.parent = _container;
        treasure.isTaken = true;
        treasure.gameObject.SetActive(false);
        treasures.Add(treasure);
        
        UpdateUI();
    }
    public Treasure Take() 
    {
        var treasure = treasures[0];
        treasure.gameObject.SetActive(true);
        treasures.Remove(treasure);
        
        UpdateUI();
        return treasure;
    }
}
