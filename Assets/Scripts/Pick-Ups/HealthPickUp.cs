using Fusion;
using UnityEngine;

public class HealthPickup : NetworkBehaviour
{
    [SerializeField] private int healthAmount = 20;
    [SerializeField] private LayerMask targetLayer; 

    private void OnTriggerEnter(Collider other)
    {
        if (!FindAnyObjectByType<NetworkRunner>().IsServer) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Health playerHealth = other.GetComponent<NetworkPlayer_Health>();
            Debug.Log("COllided with player");
            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);
                Runner.Despawn(Object);
            }
        }
    }
}
