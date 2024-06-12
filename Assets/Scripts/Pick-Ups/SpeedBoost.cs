using Fusion;
using UnityEngine;

public class SpeedBoost : NetworkBehaviour
{
    [SerializeField] private float speedBoostAmount = 2f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private AudioClip pickupSFX;
    [SerializeField] private NetworkObject pickupEffect;
    private NetworkRunner runner;

    public override void Spawned()
    {
        runner = FindAnyObjectByType<NetworkRunner>();

        if (!runner.IsServer) return;

        if (FindAnyObjectByType<RoundManager>() != null) RoundManager.Instance.ResetRound += PickedUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!runner.IsServer) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Movement playerMovement = other.GetComponent<NetworkPlayer_Movement>();
            AudioManager.Instance.PlayAudioSFX(pickupSFX, transform.position);
            if (playerMovement != null)
            {
                playerMovement.ApplySpeedBoost(speedBoostAmount, duration);
                playerMovement.Character.OnPickup(true);
                if (pickupEffect != null)
                {
                    NetworkObject pickupVFX = Runner.Spawn(pickupEffect, playerMovement.transform.position, transform.rotation);

                    if (pickupVFX != null)
                    {
                        pickupVFX.GetComponent<Transform>().SetParent(playerMovement.Character.transform);
                    }
                }
                Runner.Despawn(Object);
            }
        }
    }

    public void PickedUp()
    {
        if (FindAnyObjectByType<RoundManager>() != null) RoundManager.Instance.ResetRound -= PickedUp;
        runner.Despawn(Object);
    }
}
