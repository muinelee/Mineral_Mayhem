using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystra_Basic_Attack_Projectile : NetworkAttack_Base
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float offset;

    [Header("Lifetime Components")]
    [SerializeField] private float lifeDuration;
    private TickTimer lifeTimer = TickTimer.None;

    [Header("Other Attack Properties")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        base.Spawned();

        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(0, offset);

        Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

        transform.Translate(offsetVector);

        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeDuration);
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        DealDamage();

        if (lifeTimer.Expired(Runner))
        {
            AttackEnd();
        }
    }

    public void AttackEnd()
    {
        lifeTimer = TickTimer.None;
        Runner.Despawn(Object);
    }

    protected override void DealDamage()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
        foreach (LagCompensatedHit hit in hits)
        {
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                healthComponent.OnTakeDamage(damage);
                healthComponent.OnKnockBack(knockback, transform.position);
                AttackEnd();
            }
        }
    }
}