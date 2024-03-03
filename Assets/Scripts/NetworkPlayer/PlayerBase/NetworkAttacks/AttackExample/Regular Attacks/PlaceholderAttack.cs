using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceholderAttack : NetworkAttack_Base
{
    /* 
     Attack Description:
    
    Spawn an attack in front of the player and produce a particle effect.
    Get the objects in area of attack and (temporary) display object network ID
    */

    // Variables to destroy this gameobject
    [SerializeField] private float lifetimeDuration;
    private TickTimer timer = TickTimer.None;

    // Forward offset during spawn
    [SerializeField] private float offset;

    // Components for getting objects in attack range
    [SerializeField] private float radius;
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        timer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
        transform.position += transform.forward * offset;

        // Deal Damage Last
        DealDamage();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (timer.Expired(Runner))
        {
            timer = TickTimer.None;
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }

    protected override void DealDamage()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            Debug.Log($"Did we hit a hitbox? {hits[i].Hitbox}");
            NetworkPlayer_Health healthHandler = hits[i].GameObject.GetComponentInParent<NetworkPlayer_Health>();

            if (healthHandler) healthHandler.OnTakeDamage(damage);
        }

 /*     foreach (var hit in hits)
        {
            var healthHandler = hit.GameObject.GetComponent<NetworkHealthHandler>();

            if (healthHandler != null)
            {
                healthHandler.OnTakeDamage(damage);
            }
        } */
    }
}