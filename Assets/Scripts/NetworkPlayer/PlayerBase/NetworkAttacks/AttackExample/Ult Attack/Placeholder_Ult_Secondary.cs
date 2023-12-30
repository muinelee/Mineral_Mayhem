using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder_Ult_Secondary : NetworkAttack_Base
{
    [Header("Placeholder Primary")]
    private Placeholder_Ult_Primary parent;

    [Header("Lifetime duration")]
    [SerializeField] private float lifetimeDuration;
    private TickTimer timer = TickTimer.None;

    [Header("Movement Properties")]
    [SerializeField] private float moveSpeed;
    private NetworkTransform networkTransform;

    public override void Spawned()
    {
        timer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
        networkTransform = GetComponent<NetworkTransform>();
    }

    public override void FixedUpdateNetwork()
    {
        networkTransform.transform.Translate(transform.forward * moveSpeed * Runner.DeltaTime);
        if (timer.Expired(Runner)) Runner.Despawn(GetComponent<NetworkObject>());
    }

    public void SetParent(Placeholder_Ult_Primary primary)
    {
        parent = primary;
    }
}
