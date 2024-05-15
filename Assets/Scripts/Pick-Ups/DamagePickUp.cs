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
            CharacterEntity character = other.GetComponent<CharacterEntity>();
            if (character != null)
            {
                character.OnEnergyChange(energyAmount);
                Runner.Despawn(Object);
            }
        }
    }
}
