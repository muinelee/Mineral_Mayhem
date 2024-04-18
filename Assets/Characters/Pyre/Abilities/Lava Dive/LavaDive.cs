using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LavaDive : NetworkAttack_Base 
{
    [Header("Target Self Properties")]
    [SerializeField] private float searchRadius = 0.1f;
    [SerializeField] private LayerMask collisionLayer;

    [Header("Trail Damage Properties")]
    [SerializeField] private float diveDistance;
    [SerializeField] private float boxWidth;
    [SerializeField] private float tickRate = 0.5f;
    [SerializeField] private int tickDamage = 3;
    private TickTimer trailDamageTimer = TickTimer.None;
    private List<CharacterEntity> playerEntities = new List<CharacterEntity>();

    [Header("Attack End Damage Properties")]
    [SerializeField] private float forceForward;
    [SerializeField] private float lingerTime;
    [SerializeField] private float attackRadius = 2;
    private bool finishDive = false;
    private TickTimer lingerTimer;
    

    private CharacterEntity character;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    
    public override void Spawned()
    {
        AttackStart();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

        if (lingerTimer.Expired(Runner)) AttackEnd();

        // Ensure Trail does damage until the end
        ManageTrailDamage();

        if (finishDive) return;

        character.Rigidbody.Rigidbody.AddForce(transform.forward * forceForward);
        
        // Check if player reached max distance and let the attack linger
        if (Vector3.Magnitude(character.transform.position - transform.position) > diveDistance) DiveEnd();
    }

    private void AttackStart()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, searchRadius, player: Object.InputAuthority, hits, collisionLayer);

        for (int i = 0; i < hits.Count; i++)
        {
            character = hits[i].GameObject.GetComponentInParent<CharacterEntity>();

            if (character)
            {
                if (character.Object.InputAuthority != Object.InputAuthority) continue;

                character.Rigidbody.Rigidbody.AddForce(transform.forward * forceForward);

                // Disable collider and gravity
                character.Rigidbody.Rigidbody.useGravity = false;
                character.Collider.enabled = false;
                hits.Clear();
            }
        }

        trailDamageTimer = TickTimer.CreateFromSeconds(Runner, tickRate);
    }

    private void AttackEnd()
    {
        Debug.Log("Despawning the Lava Dive");
        Runner.Despawn(Object);
    }

    private void DiveEnd()
    {
        // Temp for showcase to Brad

        character.Animator.anim.CrossFade("LavaDiveEnd", 0.1f);

        // Temp



        DealDamage();
        lingerTimer = TickTimer.CreateFromSeconds(Runner, lingerTime);

        //character.Animator.ResetAnimation();
        finishDive = true;
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
        Vector3 midwayPoint = transform.position + ((character.transform.position - transform.position) / 2);

        // Overlap box dimensions
        float boxLength = Vector3.Magnitude(character.transform.position - transform.position);
        Vector3 boxDimensions = Vector3.right * boxWidth + Vector3.up + Vector3.forward * boxLength;

        // Find player hit and deal damage
        Runner.LagCompensation.OverlapBox(midwayPoint, boxDimensions, Quaternion.identity, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            // Apply damage
            IHealthComponent healthComponent = hits[i].GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null) healthComponent.OnTakeDamage(tickDamage);

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
                Debug.Log("Should be doing damage");
                healthComponent.OnTakeDamage(damage);

                Vector3 directionTowardsTrail = hits[i].GameObject.transform.position + (transform.position - character.transform.position).normalized - Vector3.up;

                healthComponent.OnKnockBack(knockback, directionTowardsTrail);
            }
        }
    }
}