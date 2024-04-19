using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coldmehameha : NetworkAttack_Base
{
    //componets for getting attack objects
    [Header("Attack Properties")]
    //used for OverlapSphere, will be removed when swapped to OverlapBox
    [SerializeField] private float radius; ///////////////////////////
    //lifetime of the attack
    [SerializeField] private float lifetime;
    //ticktimer to work with lifetime
    private TickTimer lifeTimer = TickTimer.None;

    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;
    public override void Spawned() {
        //call base class spawn function
        base.Spawned();
        //lifetimer ticktimer created from lifetime variable
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
    }
    void FixedUpdate()
    {
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
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);
        //loop to check for hits
        foreach (LagCompensatedHit hit in hits) {
            //get health component interface
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();
            //if health component is not null
            if (healthComponent != null) {
                //debug log for testing
                Debug.Log("Dealing Damage");
                //if health component is not dead, or if the team is the same, continue
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;
                //call the on take damage function from the health component
                healthComponent.OnTakeDamage(damage);
            }

            CharacterEntity characterEntity = hit.GameObject.GetComponentInParent<CharacterEntity>();
        }
    }
}
