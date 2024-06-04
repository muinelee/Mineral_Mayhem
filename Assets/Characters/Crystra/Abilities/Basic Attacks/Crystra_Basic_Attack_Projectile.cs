using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystra_Basic_Attack_Projectile : NetworkAttack_Base
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float offset;
    [SerializeField] private float spawnHeight;

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

        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);

        if (!Runner.IsServer) return;

        transform.position += Vector3.up * spawnHeight;
        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(0, offset);

        Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

        GetComponent<NetworkRigidbody>().Rigidbody.velocity = transform.forward * speed;

        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeDuration);

    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

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
            Runner.Spawn(this.onHitEffect, this.transform.position, Quaternion.identity);
            if (healthComponent != null)
            {
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                healthComponent.OnTakeDamage(damage, true);
                healthComponent.OnKnockBack(knockback, transform.position);
                AttackEnd();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner == null) return;

        if (!runner.IsServer) return;
        Runner.Spawn(this.onHitEffect, this.transform.position, Quaternion.identity);
        runner.Despawn(Object);
    }
}