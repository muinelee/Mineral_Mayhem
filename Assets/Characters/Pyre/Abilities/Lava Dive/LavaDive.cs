using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEditor;
using static Fusion.NetworkCharacterController;

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
    private float numFireColumns = 3f;
    private float fireColumnSpawnTime = 0.75f;

    [Header("Attack End Damage Properties")]
    [SerializeField] private float forceForward;
    [SerializeField] private float lingerTime;
    [SerializeField] private float attackRadius = 2;
    [SerializeField] private NetworkObject attackEndVFX;
    [SerializeField] private NetworkObject fireColumnVFX;
    private NetworkObject smashVFX;
    private bool finishDive = false;
    private TickTimer lingerTimer;

    [Header("Stage Collider Mask")]
    [SerializeField] private LayerMask stageLayers;

    private CharacterEntity character;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private Vector3 midwayPointRef;

    [SerializeField] AudioClip announcerVoiceLine;
    private bool voiceLinePlayed = false;

    public override void Spawned()
    {
        base.Spawned();

        fireColumnSpawnTime = dashDuration / (numFireColumns + 1);

        AttackStart();

        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);
        AudioManager.Instance.PlayAudioSFX(SFX[1], transform.position);
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // While the dash is active, spawn fire columns VFX
            if (dashTimer.IsRunning)
            {
                if (dashDuration - dashTimer.RemainingTime(Runner) > fireColumnSpawnTime)
                {
                    Runner.Spawn(fireColumnVFX, character.transform.position, Quaternion.identity);
                    dashDuration -= fireColumnSpawnTime;
                }
            }

            if (lingerTimer.Expired(Runner)) AttackEnd();

            // Ensure Trail does damage until the end
            ManageTrailDamage();

            if (finishDive) return;

            character.Rigidbody.Rigidbody.AddForce(transform.forward * forceForward);
        }

        if (finishDive) return;

        trail.position = character.transform.position;

        if (dashTimer.Expired(Runner)) dashTimer = TickTimer.None;

        if (dashTimer.IsRunning) return;

        Debug.Log("This is still  running");

        RaycastHit[] ray = Physics.SphereCastAll(trail.position, 3, transform.up, 1, stageLayers);

        if (ray.Length > 0)
        {
            for (int i = 0; i < ray.Length; i++)
            {
                Debug.Log(ray[i].transform.name);
            }            return;
        }

        else
        {
            Debug.Log("This really should run");
            DiveEnd();
        }
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
        lingerTimer = TickTimer.None;
        Runner.Despawn(smashVFX);
        Runner.Despawn(Object);
    }

    private void DiveEnd()
    {
        character.Animator.anim.CrossFade("LavaDiveEnd", 0.1f);
        AudioManager.Instance.PlayAudioSFX(SFX[2], transform.position);

        DealDamage();
        if (Runner.IsServer) smashVFX = Runner.Spawn(attackEndVFX, character.transform.position, Quaternion.identity);
        lingerTimer = TickTimer.CreateFromSeconds(Runner, lingerTime);

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
        float boxLength = Vector3.Magnitude(midwayPointRef - transform.position) * 3f;
        Vector3 boxDimensions = Vector3.right * boxWidth + Vector3.up + Vector3.forward * boxLength;

        // Find player hit and deal damage
        Runner.LagCompensation.OverlapBox(midwayPointRef, boxDimensions, transform.rotation, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            // Apply damage
            IHealthComponent healthComponent = hits[i].GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.team)) continue;

                healthComponent.OnTakeDamage(tickDamage, false);
            }

            // Apply status effect
            CharacterEntity playerHit = hits[i].GameObject.GetComponentInParent<CharacterEntity>();
            CharacterEntity characterEntity = hits[i].GameObject.GetComponentInParent<CharacterEntity>();

            // Ensure is applied only once per player
            if (playerEntities.Contains(playerHit))
            {
                playerEntities.Add(playerHit);
                ApplyStatusEffect(playerHit);
            }

            if (healthComponent.HP <= 0 && characterEntity && !voiceLinePlayed)
            {
                RPC_PlayAnnouncerVoiceLine();
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
            CharacterEntity characterEntity = hits[i].GameObject.GetComponentInParent<CharacterEntity>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.team)) continue;

                healthComponent.OnTakeDamage(damage, true);

                Vector3 playerhitPosition = hits[i].GameObject.transform.position;
                Vector3 directionTowardsTrail = playerhitPosition + (playerhitPosition - transform.position) - Vector3.up;

                healthComponent.OnKnockBack(knockback, directionTowardsTrail);
            }

            if (healthComponent.HP <= 0 && characterEntity && !voiceLinePlayed)
            {
                RPC_PlayAnnouncerVoiceLine();
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayAnnouncerVoiceLine()
    {
        AudioManager.Instance.PlayAudioSFX(announcerVoiceLine, transform.position);
    }
}