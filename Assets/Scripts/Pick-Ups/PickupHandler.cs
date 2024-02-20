using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class PickupHandler : NetworkBehaviour
{
    NetworkObject networkObject;

    PlayerRef playerThatPickedUp;
    string playerThatPickedUpName; 

    TickTimer disappearFromAllPlayersTimer = TickTimer.None;

    protected virtual void Start()
    {
        networkObject = GetComponent<NetworkObject>();
        disappearFromAllPlayersTimer = TickTimer.CreateFromSeconds(Runner, 5); // Limited Timer that 
    }

    protected virtual void OnPickup(PlayerRef playerPickedUp, string playerPickupedName)
    {
        
    }

    public override void FixedUpdateNetwork()
    { 
        if (Object.HasStateAuthority)
        {
            if (disappearFromAllPlayersTimer.Expired(Runner))
            {
                Runner.Despawn(networkObject); 
                //Stop from being triggered again
                disappearFromAllPlayersTimer = TickTimer.None;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Runner.Despawn(networkObject); 
    }
}
