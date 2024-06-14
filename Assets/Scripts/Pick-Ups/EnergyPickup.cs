using Fusion;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnergyPickup : NetworkBehaviour
{
    [SerializeField] private int energyAmount = 100;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private AudioClip pickupSFX;
    [SerializeField] private NetworkObject pickupEffect;

    public override void Spawned()
    {
        if (!Runner.IsServer) return;

        RoundManager.Instance.ResetRound += PickedUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!FindObjectOfType<NetworkRunner>().IsServer) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            AudioManager.Instance.PlayAudioSFX(pickupSFX, transform.position);
            NetworkPlayer_Energy playerEnergy = other.GetComponent<NetworkPlayer_Energy>();

            if (playerEnergy != null)
            {
                playerEnergy.AddEnergy(energyAmount);
                playerEnergy.Character.OnPickup(false);
                if (pickupEffect != null)
                {
                    NetworkObject pickupVFX = Runner.Spawn(pickupEffect, playerEnergy.transform.position, transform.rotation);

                    if (pickupVFX != null)
                    {
                        pickupVFX.GetComponent<Transform>().SetParent(playerEnergy.Character.transform);
                    }
                }
                PickedUp();
            }
        }
    }

    public void PickedUp()
    {
        if (!Runner.IsServer) return;
        
        RoundManager.Instance.ResetRound -= PickedUp;
        Runner.Despawn(Object);
    }
}
