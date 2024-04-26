using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healthAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkPlayer_Health playerHealth = other.GetComponent<NetworkPlayer_Health>();
            Debug.Log("COllided with player");
            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);
                Destroy(gameObject);
            }
        }
    }
}
