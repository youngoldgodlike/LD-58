using System;
using Main.Scripts;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public Transform _teleportPoint;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            player.TeleportTo(_teleportPoint);
    }
}
