using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlam : NetworkAttack_Base
{
    [Header("Spawned properties")]
    [SerializeField] private float offset;
    [SerializeField] private float length = 3f;
    [SerializeField] private float width = 3f;

    [Header("Attack Properties")]
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    [Header("Lifetime properties")]
    [SerializeField] private float lifetimeDuration;
    private TickTimer lifeTimer = TickTimer.None;

    public override void Spawned()
    {
        base.Spawned();

        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);

        if (!Runner.IsServer) return;

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

        Vector3 boxSize = new Vector3(width, 1, length);

        Runner.LagCompensation.OverlapBox(transform.position + (transform.forward * offset), boxSize, transform.rotation, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);

        foreach (LagCompensatedHit hit in hits)
        {
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                healthComponent.OnTakeDamage(damage);
                healthComponent.OnKnockBack(knockback, transform.position);
            }

            StatusHandler enemyStatusHandlers = hit.GameObject.GetComponentInParent<StatusHandler>();

            if (enemyStatusHandlers != null)
            {
                foreach (StatusEffect status in statusEffectSO)
                {
                    enemyStatusHandlers.AddStatus(status);
                }
            }

            // Delete later

            else Debug.Log("Nothing to apply Status Effect");
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new Vector3(width, 1, length);

        Gizmos.DrawWireCube(transform.position + (transform.forward * offset), size);
    }
}
