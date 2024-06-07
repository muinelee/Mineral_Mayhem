using Fusion;
using UnityEngine;

public class HealthPickup : NetworkBehaviour
{
    [SerializeField] private int healthAmount = 20;
    [SerializeField] private LayerMask targetLayer;

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
            NetworkPlayer_Health playerHealth = other.GetComponent<NetworkPlayer_Health>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);
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
