using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderAttack : NetworkAttack_Base
{
    /* 
     Attack Description:
    
        Spawn an attack in front of the player and produce a particle effect.
        Get the objects in area of attack and (temporary) display object network ID
    */
    [SerializeField] private float lifetimeDuration;
    private TickTimer timer = TickTimer.None;

    public override void Spawned()
    {
        timer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (timer.Expired(Runner))
            {
                timer = TickTimer.None;
                Runner.Despawn(GetComponent<NetworkObject>());
            }
        }
    }
}
