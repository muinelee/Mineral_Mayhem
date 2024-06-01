using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class VineGround : NetworkAttack_Base
{
    [SerializeField] private float radius;

    [SerializeField] private float lifetimeDuration = 5f;
    private TickTimer lifeTimer;

    [SerializeField] private float tickRate;
    private TickTimer tickRateTimer = TickTimer.None;

    // Player hit variables
    private List<StatusHandler> playersHit = new List<StatusHandler>();
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        base.Spawned();

        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);
        //AudioManager.Instance.PlayAudioSFX(SFX[1], transform.position);

        tickRateTimer = TickTimer.CreateFromSeconds(Runner, tickRate);
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!Runner.IsServer) return;

        if (tickRateTimer.Expired(Runner))
        {
            DealDamage();
            tickRateTimer = TickTimer.CreateFromSeconds(Runner, tickRate);
        }

        if (lifeTimer.Expired(Runner))
        {
            tickRateTimer = TickTimer.None;
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
            // Deal damage to all things in radius
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                healthComponent.OnTakeDamage(damage, true);
            }

            // Apply status effect once per enemy player hit
            StatusHandler playerHitStatusHandler = hit.GameObject.GetComponentInParent<StatusHandler>();

            if (playerHitStatusHandler != null)
            {
                if (playersHit.Contains(playerHitStatusHandler)) continue;

                foreach (StatusEffect status in statusEffectSO)
                {
                    playerHitStatusHandler.AddStatus(status);
                    playersHit.Add(playerHitStatusHandler);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
