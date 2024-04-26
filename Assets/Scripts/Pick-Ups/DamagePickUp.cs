using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePickup : MonoBehaviour
{
    [SerializeField] private int damageAmount = 20;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
