using Fusion;
using UnityEngine;

public class EnergyPickup : NetworkBehaviour
{
    [SerializeField] private int energyAmount = 100;
    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Energy playerEnergy = other.GetComponent<NetworkPlayer_Energy>();
            Debug.Log("Collided with Player");
            if (playerEnergy != null )
            {
                playerEnergy.AddEnergy(energyAmount);
                Runner.Despawn(Object);
            }
            
        }
    }
}
