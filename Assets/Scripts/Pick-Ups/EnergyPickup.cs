using Fusion;
using UnityEngine;

public class EnergyPickup : NetworkBehaviour
{
    [SerializeField] private int energyAmount = 100;
    [SerializeField] private LayerMask targetLayer;

    public override void Spawned()
    {
        if (!Runner.IsServer) return;

        if (FindAnyObjectByType<RoundManager>() != null) RoundManager.Instance.ResetRound += PickedUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!FindObjectOfType<NetworkRunner>().IsServer) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Energy playerEnergy = other.GetComponent<NetworkPlayer_Energy>();

            if (playerEnergy != null )
            {
                playerEnergy.AddEnergy(energyAmount);
                playerEnergy.Character.OnPickup(false);
                PickedUp();
            }
        }
    }

    public void PickedUp()
    {
        if (FindAnyObjectByType<RoundManager>() != null) RoundManager.Instance.ResetRound -= PickedUp;
        Runner.Despawn(Object);
    }
}
