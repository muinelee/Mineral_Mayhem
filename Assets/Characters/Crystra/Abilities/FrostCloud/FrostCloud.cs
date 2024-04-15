using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class FrostCloud : NetworkAttack_Base
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private float maxTravelDistance;
    private float distanceTravelled = 0;
    [SerializeField] private Vector3 offset;

    [Header("Lifetime Components")]
    //[SerializeField] private float lifeDuration;
    private TickTimer lifeTimer = TickTimer.None;

    [Header("Other Attack Properties")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();


    public override void Spawned()
    {
        base.Spawned();

        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
        {
            AttackEnd();
            return;
        }

        DealDamage();

        float moveDistance = speed * Runner.DeltaTime;

        if (distanceTravelled + moveDistance > maxTravelDistance) return;        

        distanceTravelled += moveDistance;
    }

    public void AttackEnd()
    {
        lifeTimer = TickTimer.None;
        Runner.Despawn(Object);
    }

    protected override void DealDamage()
    {
        if (!Runner) return;

        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
        foreach (LagCompensatedHit hit in hits)
        {
<<<<<<< Updated upstream
=======

            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                Debug.Log("Frost Cloud Deal Damage is being called");
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                healthComponent.OnTakeDamage(damage);
            }

>>>>>>> Stashed changes
            CharacterEntity characterEntity = hit.GameObject.GetComponentInParent<CharacterEntity>();

            if (characterEntity)
            {
                if (characterEntity.Health.isDead || CheckIfSameTeam(characterEntity)) continue;

                characterEntity.Health.OnHit(damage);

                if (statusEffectSO.Count > 0 && characterEntity)
                {
                    foreach (StatusEffect status in statusEffectSO)
                    {
                        characterEntity.OnStatusBegin(status);
                    }
                }
            }
        }
    }
}
