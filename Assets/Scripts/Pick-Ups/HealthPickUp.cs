using Fusion;
using UnityEngine;

public class HealthPickup : NetworkBehaviour
{
    [SerializeField] private int healthAmount = 20;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private AudioClip pickupSFX;
    [SerializeField] private NetworkObject pickupEffect;

    public override void Spawned()
    {
        if (!Runner.IsServer) return;

        if (RoundManager.Instance != null) RoundManager.Instance.ResetRound += PickedUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!FindAnyObjectByType<NetworkRunner>().IsServer) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            AudioManager.Instance.PlayAudioSFX(pickupSFX, transform.position);
            CharacterEntity character = other.GetComponent<CharacterEntity>();

            if (character != null)
            {
                character.OnHeal(healthAmount);
                if (pickupEffect != null)
                {
                    NetworkObject pickupVFX = Runner.Spawn(pickupEffect, character.transform.position, transform.rotation);
                    
                    if (pickupVFX != null)
                    {
                        pickupVFX.GetComponent<Transform>().SetParent(character.transform);
                    }
                }
                PickedUp();
            }
        }
    }

    public void PickedUp()
    {
        if (RoundManager.Instance != null) RoundManager.Instance.ResetRound -= PickedUp;
        Runner.Despawn(Object);
    }
}
