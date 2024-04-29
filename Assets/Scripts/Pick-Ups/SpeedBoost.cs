using Fusion;
using UnityEngine;

public class SpeedBoost : NetworkBehaviour
{
    [SerializeField] private float speedBoostAmount = 2f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (!Runner.IsServer) return;

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Movement playerMovement = other.GetComponent<NetworkPlayer_Movement>();
            if (playerMovement != null)
            {
                playerMovement.ApplySpeedBoost(speedBoostAmount, duration);

                Runner.Despawn(Object);
            }
        }
    }
}
