using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; private set; }


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Debug.Log("Spawned local player");
        }
        else Debug.Log("Spawned remote player");
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
