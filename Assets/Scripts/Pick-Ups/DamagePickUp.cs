using Fusion;
using UnityEngine;

public class DamagePickup : NetworkBehaviour
{
    [SerializeField] private int energyAmount = 20;
    [SerializeField] private LayerMask targetLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (!FindAnyObjectByType<NetworkRunner>().IsServer) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Energy playerEnergy = other.GetComponent<NetworkPlayer_Energy>();
            if (playerEnergy != null)
            {
                playerEnergy.AddEnergy(energyAmount);
                Runner.Despawn(Object);
            }
        }
    }
}
