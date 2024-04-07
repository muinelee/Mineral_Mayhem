using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;
using static Fusion.NetworkCharacterController;

public class Test_RapidIceShot_IceSpike : NetworkBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float offset;

    [Header("Lifetime Properties")]
    [SerializeField] private float lifetimeDuration;
    private TickTimer lifetimeTimer = TickTimer.None;

    [Header("Damage Properties")]
    [SerializeField] private float damage;
    [SerializeField] private float radius;
    [SerializeField] private float knockback;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(0, offset);

        Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

        transform.Translate(offsetVector);

        lifetimeTimer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
    }

    public override void FixedUpdateNetwork()
    {
        // move
        transform.Translate(Vector3.forward * speed);

        CheckForHits();

        // manage lifetime
        if (lifetimeTimer.Expired(Runner))
        {
            EndAttack();
        }
    }

    public void CheckForHits()
    {
        // search for objects hit and deal damage if necessary
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);

        if (hits.Count == 0) return;

        foreach (LagCompensatedHit hit in hits)
        {
            CharacterEntity character = hit.GameObject.GetComponentInParent<CharacterEntity>();

            if (!character) continue;

            character.OnHit(damage);
            character.Health.OnKnockback(knockback, transform.position);
            Debug.Log("Should be applying knockback");

            EndAttack();
        }
    }

    public void EndAttack()
    {
        lifetimeTimer = TickTimer.None;
        Runner.Despawn(Object);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);
    }
}
