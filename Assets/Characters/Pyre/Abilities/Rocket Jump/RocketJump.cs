using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class RocketJump : NetworkAttack_Base
{
    private Transform rigTransform;

    [Header("Target Self Properties")]
    [SerializeField] private float searchRadius;    
    [SerializeField] private LayerMask collisionLayer;
    private CharacterEntity character;

    [Header("Attack Properties")]
    [SerializeField] private float attackRadius;
    [SerializeField] private float forceForward;
    [SerializeField] private NetworkObject jumpVFX;
    [SerializeField] private NetworkObject slamVFX;
    [SerializeField] private float vfxDuration = 5;
    private NetworkObject jumpVFXInstance;
    private NetworkObject slamVFXInstance;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private float startHeight = 0;

    private TickTimer attackLifeTimer = TickTimer.None;

    public override void Spawned()
    {
        base.Spawned();
        
        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);
        
        if (!Runner.IsServer) return;

        AttackStart();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

        if (rigTransform.position.y < startHeight && !attackLifeTimer.IsRunning)
        {
            //Spawn Particles
            slamVFXInstance = Runner.Spawn(slamVFX, rigTransform.position, rigTransform.rotation);
            RPC_RocketJumpLandingSFX();
            attackLifeTimer = TickTimer.CreateFromSeconds(Runner, vfxDuration);

            DealDamage();

            //Enable colliders
            character.Rigidbody.Rigidbody.useGravity = true;
        }

        if (attackLifeTimer.Expired(Runner)) AttackEnd();
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

                // Set components
                jumpVFXInstance = Runner.Spawn(jumpVFX, transform.position, transform.rotation);
                rigTransform = hits[i].GameObject.transform.GetChild(0);
                startHeight = rigTransform.position.y;
                
                // Apply force
                character.Rigidbody.Rigidbody.AddForce(transform.forward * forceForward);

                // Disable collider and gravity
                character.Rigidbody.Rigidbody.useGravity = false;
                hits.Clear();
            }
        }
    }

    private void AttackEnd()
    {
        attackLifeTimer = TickTimer.None;
        Runner.Despawn(jumpVFXInstance);
        Runner.Despawn(slamVFXInstance);
        Runner.Despawn(Object);
    }

    protected override void DealDamage()
    {
        Runner.LagCompensation.OverlapSphere(rigTransform.position, attackRadius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        foreach (LagCompensatedHit hit in hits)
        {
            IHealthComponent playerHit = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (playerHit == null || CheckIfSameTeam(playerHit.team)) continue;

            playerHit.OnTakeDamage(damage);
            playerHit.OnKnockBack(knockback, rigTransform.position);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_RocketJumpLandingSFX()
    {
        AudioManager.Instance.PlayAudioSFX(this.SFX[1], transform.position);
    }
}