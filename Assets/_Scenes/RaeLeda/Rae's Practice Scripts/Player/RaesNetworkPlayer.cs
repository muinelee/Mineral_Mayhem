using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; 

public class RaesNetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static RaesNetworkPlayer Local { get; set; } 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority) // It will run on every object if this isnt checked
        {
            Local = this;
            Debug.Log("Spawned Local Player");
        }

        else Debug.Log("Spawned Remote player"); 
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object); 
        }
    }
}
