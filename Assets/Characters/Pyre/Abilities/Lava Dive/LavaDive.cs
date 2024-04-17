using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LavaDive : NetworkAttack_Base 
{
    [Header("Target Self Properties")]
    [SerializeField] private float searchRadius = 0.1f;
    [SerializeField] private LayerMask collisionLayer;

    [Header("Attack Properties")]
    [SerializeField] private float forceForward;
    [SerializeField] private float diveDistance;
    [SerializeField] private float boxWidth;
    private bool finishDive = false;

    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private CharacterEntity character;

    public override void Spawned()
    {
        AttackStart();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

        if (finishDive) return;

        character.Rigidbody.Rigidbody.AddForce(transform.forward * forceForward);

        if (Vector3.Magnitude(character.transform.position - transform.position) > diveDistance)
        {
            DiveEnd();
        }
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
    }

    private void DiveEnd()
    {
        character.Animator.ResetAnimation();
        finishDive = true;
    }

    protected override void DealDamage()
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
            IHealthComponent playerHit = hits[i].GameObject.GetComponentInParent<IHealthComponent>();

            if (playerHit == null || CheckIfSameTeam(playerHit.team)) continue;

            playerHit.OnTakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 midwayPoint = transform.position + ((character.transform.position - transform.position) / 2);

        // Overlap box dimensions
        float boxLength = Vector3.Magnitude(character.transform.position - transform.position);
        Vector3 boxDimensions = Vector3.right * boxWidth + Vector3.up + Vector3.forward * boxLength;

        Gizmos.DrawWireCube(midwayPoint, boxDimensions);
    }
}