using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Template : NetworkAttack_Base
{
    [Header("Lifetime Properties")]
    [SerializeField] private float lifetimeDuration;
    private TickTimer lifetimeTimer = TickTimer.None;

    // Components for getting objects in attack range
    [SerializeField] private float radius;
    [SerializeField] private float positionOffset;
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;

    // Start is called before the first frame update
    public override void Spawned()
    {
        transform.position += transform.forward * positionOffset;
        DealDamage();
        lifetimeTimer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifetimeTimer.Expired(Runner)) Runner.Despawn(GetComponent<NetworkObject>());
    }

    private void DealDamage()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            Debug.Log($"Did we hit a hitbox? {hits[i].Hitbox}");
            NetworkPlayer_Health healthHandler = hits[i].GameObject.GetComponentInParent<NetworkPlayer_Health>();

            if (healthHandler) healthHandler.OnTakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}