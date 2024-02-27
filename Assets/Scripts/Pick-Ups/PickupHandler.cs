using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class PickupHandler : NetworkBehaviour
{
    NetworkObject networkObject;
    TickTimer disappearFromAllPlayersTimer = TickTimer.None;

    [Header("Speed Components")] 
    [SerializeField] private float speedBoostAmount = 2f;
    [SerializeField] private float duration = 5f;

    [Header("Health Components")] 
    [SerializeField] private int healthAmount = 20;

    private bool isPickedUp = false; 
    public enum Pickups
    {
        Health,
        Speed,
        Damage
    }

    public Pickups pickups; 

    protected virtual void Start()
    {
        networkObject = GetComponent<NetworkObject>();
        disappearFromAllPlayersTimer = TickTimer.CreateFromSeconds(Runner, 5); // Limited Timer that 
    }

    public override void FixedUpdateNetwork()
    { 
        if (Object.HasStateAuthority && !isPickedUp)
        {
            if (disappearFromAllPlayersTimer.Expired(Runner))
            {
                Runner.Despawn(networkObject); 
                //Stop from being triggered again
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        // if we're in the server (then these will be called) 
        // if not, do nothing 
        //if (Runner.IsServer)  
        if (Object.HasStateAuthority && !isPickedUp)
        {
            isPickedUp = true; 
            if (pickups == Pickups.Health)
            {
                NetworkPlayer_Health playerHealth = other.GetComponent<NetworkPlayer_Health>();
                if (playerHealth != null) playerHealth.Heal(healthAmount);

            }

            if (pickups == Pickups.Speed)
            {
                NetworkPlayer_Movement playerMovement = other.GetComponent<NetworkPlayer_Movement>();
                if (playerMovement != null) playerMovement.ApplySpeedBoost(speedBoostAmount, duration);

            }

            if (pickups == Pickups.Damage)
            {
                NetworkPlayer_Health playerHealth = other.GetComponent<NetworkPlayer_Health>();
                if (playerHealth != null) playerHealth.OnTakeDamage(healthAmount);
            }

            Debug.Log("Despawn Object");
            Runner.Despawn(networkObject);
        }
    }
}
