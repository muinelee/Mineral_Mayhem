using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class LavaDive : NetworkAttack_Base 
{
    [Header("Target Self Properties")]
    [SerializeField] private float searchRadius = 0.1f;
    [SerializeField] private LayerMask collisionLayer;

    [Header("Trail Damage Properties")]
    [SerializeField] private float boxWidth;
    [SerializeField] private float tickRate = 0.5f;
    [SerializeField] private int tickDamage = 3;
    [SerializeField] private Transform trail;
    [SerializeField] private float dashDuration;
    private TickTimer dashTimer = TickTimer.None;
    private TickTimer trailDamageTimer = TickTimer.None;
    private List<CharacterEntity> playerEntities = new List<CharacterEntity>();

    [Header("Attack End Damage Properties")]
    [SerializeField] private float forceForward;
    [SerializeField] private float lingerTime;
    [SerializeField] private float attackRadius = 2;
    [SerializeField] private NetworkObject attackEndVFX;
    private bool finishDive = false;
    private TickTimer lingerTimer;

    private CharacterEntity character;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private Vector3 midwayPointRef;

    public override void Spawned()
    {
        AttackStart();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (lingerTimer.Expired(Runner)) AttackEnd();

            // Ensure Trail does damage until the end
            ManageTrailDamage();

            if (finishDive) return;

            character.Rigidbody.Rigidbody.AddForce(transform.forward * forceForward);
        }

        if (finishDive) return;
        if (dashTimer.Expired(Runner)) DiveEnd();
        trail.position = character.transform.position;
    }

    private void AttackStart()
    {
        TrailRenderer tr = trail.GetComponent<TrailRenderer>();
        tr.startWidth = boxWidth;
        tr.endWidth = boxWidth;

        Runner.LagCompensation.OverlapSphere(transform.position, searchRadius, player: Object.InputAuthority, hits, collisionLayer);

        for (int i = 0; i < hits.Count; i++)
        {
            character = hits[i].GameObject.GetComponentInParent<CharacterEntity>();

            if (character)
            {
                if (character.Object.InputAuthority != Object.InputAuthority) continue;

                character.Rigidbody.Rigidbody.AddForce(transform.forward * forceForward);

                character.Collider.enabled = false;
                hits.Clear();
            }
        }

        dashTimer = TickTimer.CreateFromSeconds(Runner, dashDuration);
        trailDamageTimer = TickTimer.CreateFromSeconds(Runner, tickRate);
    }

    private void AttackEnd()
    {
        Runner.Despawn(Object);
    }

    private void DiveEnd()
    {
        character.Animator.anim.CrossFade("LavaDiveEnd", 0.1f);

        DealDamage();
        if (Runner.IsServer) Runner.Spawn(attackEndVFX, character.transform.position, Quaternion.identity);
        lingerTimer = TickTimer.CreateFromSeconds(Runner, lingerTime);

        //character.Animator.ResetAnimation();
        finishDive = true;
        character.Collider.enabled = true;
    }
    private void ManageTrailDamage()
    {
        if (!trailDamageTimer.Expired(Runner)) return;

        trailDamageTimer = TickTimer.None;
        trailDamageTimer = TickTimer.CreateFromSeconds(Runner, tickRate);

        TrailDamage();
    }

    private void TrailDamage()
    {
        // Midpoint to player for overlapbox position
        if (!finishDive) midwayPointRef = transform.position + ((character.transform.position - transform.position) / 2);
        
        // Overlap box dimensions
        float boxLength = Vector3.Magnitude(midwayPointRef - transform.position) * 2;
        Vector3 boxDimensions = Vector3.right * boxWidth + Vector3.up + Vector3.forward * boxLength;

        // Find player hit and deal damage
        Runner.LagCompensation.OverlapBox(midwayPointRef, boxDimensions, Quaternion.identity, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            // Apply damage
            IHealthComponent healthComponent = hits[i].GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.team)) continue;
                healthComponent.OnTakeDamage(tickDamage);
            }

            // Apply status effect
            CharacterEntity playerHit = hits[i].GameObject.GetComponentInParent<CharacterEntity>();

            // Ensure is applied only once per player
            if (playerEntities.Contains(playerHit))
            {
                playerEntities.Add(playerHit);
                ApplyStatusEffect(playerHit);
            }
        }
    }

    private void ApplyStatusEffect(CharacterEntity playerHit)
    {
        if (playerHit == null || CheckIfSameTeam(playerHit.Health.team)) return;

        if (statusEffectSO.Count > 0 && playerHit)
        {
            foreach (StatusEffect status in statusEffectSO)
            {
                playerHit.OnStatusBegin(status);
            }
        }
    }

    protected override void DealDamage()
    {
        Runner.LagCompensation.OverlapSphere(character.transform.position, attackRadius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            IHealthComponent healthComponent = hits[i].GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.team)) continue;

                healthComponent.OnTakeDamage(damage);

                Vector3 playerhitPosition = hits[i].GameObject.transform.position;
                Vector3 directionTowardsTrail = playerhitPosition + (playerhitPosition - transform.position) - Vector3.up;

                healthComponent.OnKnockBack(knockback, directionTowardsTrail);
            }
        }
    }
}