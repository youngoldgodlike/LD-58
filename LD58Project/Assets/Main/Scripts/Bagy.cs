using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Bagy : MonoBehaviour {
    [SerializeField] Transform _container;
    public List<Treasure> treasures = new();

    [SerializeField] private int _capacity = 10;
    [SerializeField] private Image _progressBarFill;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pickUpClip;

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

        var randomPitch = Random.Range(0.9f, 1.1f);
        _audioSource.pitch = randomPitch;
        
        _audioSource.PlayOneShot(_pickUpClip);
        
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
