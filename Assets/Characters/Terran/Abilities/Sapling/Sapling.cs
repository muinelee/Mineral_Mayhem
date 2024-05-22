using Fusion;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkCharacterController;

public class Sapling : NetworkAttack_Base
{
    [Header("Spawn Properties")]
    [SerializeField] private float offset;

    [Header("Start Up Properties")]
    [SerializeField] private float startUpDuration = 1f;
    private TickTimer startUpTimer = TickTimer.None;

    [Header("Movement properties")]
    [SerializeField] private float speed;
    private NetworkRigidbody rb;

    [Header("Fuse Properties")]
    [SerializeField] private float fuseDuration = 0.3f;
    private TickTimer fuseTimer = TickTimer.None;

    [Header("Attack Properties")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        base.Spawned();

        transform.position += transform.forward * offset;

        startUpTimer = TickTimer.CreateFromSeconds(Runner, startUpDuration);

        rb = GetComponent<NetworkRigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!Runner.IsServer) return;

        if (startUpTimer.Expired(Runner)) startUpTimer = TickTimer.None;
        // In start up time
        else if (startUpTimer.IsRunning) return;
        
        // Move until the fuse is started
        if (!fuseTimer.IsRunning) rb.Rigidbody.AddForce(transform.forward * speed);

        if (fuseTimer.Expired(Runner))
        {
            fuseTimer = TickTimer.None;
            DealDamage();
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

    private void OnTriggerEnter(Collider other)
    {
        if (!FindObjectOfType<NetworkRunner>().IsServer) return;

        IHealthComponent healthComponent = other.GetComponentInParent<IHealthComponent>();

        if (healthComponent != null)
        {
            if (CheckIfSameTeam(healthComponent.GetTeam())) return;
        }

        rb.Rigidbody.drag = 5;
        fuseTimer = TickTimer.CreateFromSeconds(Runner, fuseDuration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}