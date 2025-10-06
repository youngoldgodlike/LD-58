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
    [SerializeField] private Image _flowerFill;
    [SerializeField] private Image _meatFill;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pickUpClip;

    private int _meatCount = 0;
    private int _flowerCount = 0;
    
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
        var meatValue = (float)_meatCount / 10;
        var flowerValue = (float)_flowerCount / 3;

        _meatFill.fillAmount = meatValue;
        _flowerFill.fillAmount = flowerValue;
    }

    private void Put(Treasure treasure)
    {
        if (treasure.isMeat)
        {
            if (_meatCount >= 10) return;
            
            treasure.transform.parent = _container;
            var randomPitch = Random.Range(0.9f, 1.1f);
            treasure.isTaken = true;
            _audioSource.pitch = randomPitch;
            _audioSource.PlayOneShot(_pickUpClip);

            treasure.gameObject.SetActive(false);

            _meatCount++;
        }
        else
        {
            if (_flowerCount >= 3) return;
            
            treasure.transform.parent = _container;
            var randomPitch = Random.Range(0.9f, 1.1f);
            treasure.isTaken = true;
            _audioSource.pitch = randomPitch;
            _audioSource.PlayOneShot(_pickUpClip);

            treasure.gameObject.SetActive(false);

            _flowerCount++;
        }
        
        
        treasures.Add(treasure);
        UpdateUI();
    }
    public Treasure Take() 
    {
        var treasure = treasures[0];
        treasure.gameObject.SetActive(true);
        treasures.Remove(treasure);

        if (treasure.isMeat) _meatCount--;
        else _flowerCount--;
        
        
        UpdateUI();
        return treasure;
    }
}
