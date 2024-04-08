using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystra_Basic_Attack_Projectile : NetworkBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float offset;

    [Header("Lifetime Components")]
    [SerializeField] private float lifeDuration;
    private TickTimer lifeTimer = TickTimer.None;

    [Header("Attack Properties")]
    [SerializeField] private float damage;
    [SerializeField] private float knockBack;
    [SerializeField] private float radius;
    [SerializeField] private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask playerLayer;

    public override void Spawned()
    {
        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(0, offset);

        Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

        transform.Translate(offsetVector);

        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeDuration);
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        // move
        transform.Translate(Vector3.forward * speed);

        CheckHits();

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
    public void CheckHits()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
        foreach (LagCompensatedHit hit in hits)
        {
            CharacterEntity characterEntity = hit.GameObject.GetComponentInParent<CharacterEntity>();

            if (characterEntity)
            {
                if (characterEntity.Health.isDead) continue;

                characterEntity.Health.OnHit(damage);
                characterEntity.Health.OnKnockBack(knockBack, transform.position);
                AttackEnd();
            }
        }
    }
}