using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Template : NetworkAttack_Base
{
    [Header("Attack Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask collisionLayer;
    private List<LagCompensatedHit> hits;

    // Components
    private NetworkRigidbody networkRB;

    public override void Spawned()
    {
        networkRB= GetComponent<NetworkRigidbody>();

        networkRB.Rigidbody.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            Debug.Log($"Did we hit a hitbox? {hits[i].Hitbox}");
            NetworkPlayer_Health healthHandler = hits[i].GameObject.GetComponentInParent<NetworkPlayer_Health>();

            if (healthHandler)
            {
                healthHandler.OnTakeDamage(damage);
            }
            
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
