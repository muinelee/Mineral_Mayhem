using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coldmehameha : NetworkAttack_Base
{
    //componets for getting attack objects
    [Header("Attack Properties")]
    //lifetime of the attack
    [SerializeField] private float lifetime;
    //ticktimer to work with lifetime
    private TickTimer lifeTimer = TickTimer.None;
    //spawn offset

    [Header("Spawn Properties")]
    public int offset;

    CharacterEntity character;
    //extends for the overlap box
    public Vector3 extends = new Vector3(0.5f, 0.5f, 6f);
    //list for hits
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;

    [SerializeField] AudioClip announcerVoiceLine;

    public override void Spawned() {
        //call base class spawn function
        base.Spawned();

        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);
        //AudioManager.Instance.PlayAudioSFX(SFX[1], transform.position);

        //lifetimer ticktimer created from lifetime variable
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);

        if (Runner.IsServer)
        {
            //call sphere to check player
            Runner.LagCompensation.OverlapSphere(transform.position, 0.1f, player: Object.InputAuthority, hits, collisionLayer);
            //loop for hits
            for (int i = 0; i < hits.Count; i++) {
                //get character entity as parent of hit object
                character = hits[i].GameObject.GetComponentInParent<CharacterEntity>();
                //set parent of the object to the character entity
                this.transform.SetParent(character.transform);
            }
        }
        //offset spawn position to match hands
        transform.position += transform.forward * offset;
    }
    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;
        //if the lifetime of the attack has expired
        if (lifeTimer.Expired(Runner)) {
            //end the attack
            AttackEnd();
            //return out of the function
            return;
        }
        //deal damage
        DealDamage();
    }

    private void AttackEnd() {
        //set life timer to none
        lifeTimer = TickTimer.None;
        //despawn the object
        Runner.Despawn(Object);
    }

    protected override void DealDamage()
    {
        if (!Runner.IsServer) return;

        //get the character rotation
        Quaternion rotation = Quaternion.LookRotation(character.transform.forward);
        //signature for hits using the characterposition + (rotation * 3(half size of the box)
        Runner.LagCompensation.OverlapBox((transform.position + (transform.forward * extends.z)), extends, transform.rotation, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);
        //loop to check for hits
        foreach (LagCompensatedHit hit in hits) {

            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();
            //if health component is not null
            if (healthComponent != null) {

                //if health component is not dead, or if the team is the same, continue
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;
                //call the on take damage function from the health component
                healthComponent.OnTakeDamage(damage, true);
                //apply knockback when hit
                healthComponent.OnKnockBack(knockback, this.transform.position);
            }

            CharacterEntity characterEntity = hit.GameObject.GetComponentInParent<CharacterEntity>();

            if (statusEffectSO.Count > 0 && characterEntity) {
                foreach (StatusEffect status in statusEffectSO) {
                    characterEntity.OnStatusBegin(status);
                }
            }

            if (healthComponent.HP <= 0)
            {
                RPC_PlayAnnouncerVoiceLine();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((transform.position + (transform.forward * (extends.z / 2))), extends);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayAnnouncerVoiceLine()
    {
        AudioManager.Instance.PlayAudioSFX(announcerVoiceLine, transform.position);
    }
}
