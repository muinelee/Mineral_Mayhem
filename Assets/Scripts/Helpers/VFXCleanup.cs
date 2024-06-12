using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXCleanup : NetworkBehaviour
{
    public float timer = 3f;
    private float currentTime = 0f;
    public override void FixedUpdateNetwork()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= timer && Runner.IsServer) Runner.Despawn(this.Object);
    }
}
