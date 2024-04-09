using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePickup : MonoBehaviour
{
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private LayerMask targetLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            NetworkPlayer_Health playerHealth = other.GetComponent<NetworkPlayer_Health>();
            if (playerHealth != null)
            {
                playerHealth.OnTakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }
}
