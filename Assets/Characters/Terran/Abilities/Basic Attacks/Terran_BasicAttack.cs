using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terran_BasicAttack : NetworkAttack_Base
{
    [Header("Spawned properties")]
    [SerializeField] private float offset;
    
    [Header("Attack Properties")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    [Header("Lifetime properties")]
    [SerializeField] private float lifetimeDuration;
    private TickTimer lifeTimer = TickTimer.None;



    public override void Spawned()
    {
        base.Spawned();

        //AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);

        transform.position += transform.forward * offset;

        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);

        DealDamage();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!Runner.IsServer) return;

        if (lifeTimer.Expired(Runner))
        {
            lifeTimer = TickTimer.None;
            Runner.Despawn(Object);
        }
    }

    protected override void DealDamage()
    {
        if (!Runner.IsServer) return;

        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);

        foreach (LagCompensatedHit hit in hits)
        {
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                healthComponent.OnTakeDamage(damage);
                healthComponent.OnKnockBack(knockback, transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
