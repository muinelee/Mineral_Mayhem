using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkPlayer_Block : CharacterComponent
{
    // Blocking will reduce incoming damage by a percentage. Need to access NetworkPlayer_Health to apply the damage reduction.
    [SerializeField] private float blockDamageReduction = 0.5f;

    // Shield Meter properties
    [SerializeField] private float shieldMeter = 100;

    // Control variables
    public bool canBlock = true;
    public bool isBlocking = false;

    // [Header("Block Properties")]
    // [SerializeField] private SO_Block block;

    public override void Spawned()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input) && canBlock)
        {
            //if (input.IsDown(NetworkInputData.ButtonBlock) && playerEnergy.IsUltCharged()) ActivateBlock();
        }
    }


    public void FireBlock()
    {
        if (!Object.HasStateAuthority) return;

        //Runner.Spawn(block.GetBlockPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }
}
