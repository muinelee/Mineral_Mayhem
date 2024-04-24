using Fusion;
using UnityEngine;

public class DamagePickup : NetworkBehaviour
{
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private LayerMask targetLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Health playerHealth = other.GetComponent<NetworkPlayer_Health>();
            if (playerHealth != null)
            {
                playerHealth.OnTakeDamage(damageAmount);
                Runner.Despawn(Object);
            }
        }
    }
}
