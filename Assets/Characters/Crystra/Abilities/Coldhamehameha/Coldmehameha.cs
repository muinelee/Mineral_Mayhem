using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coldmehameha : NetworkAttack_Base
{
    //componets for getting attack objects
    [SerializeField] private float radius;
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;
    private void Start()
    {

    }
    void FixedUpdate()
    {
        DealDamage();
    }
    protected override void DealDamage()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);
        //loop to check for hits
        for (int i = 0; i < hits.Count; i++)
        {
            Debug.Log($"Did we hit a hitbox? {hits[i].Hitbox}");
            NetworkPlayer_Health healthHandler = hits[i].GameObject.GetComponentInParent<NetworkPlayer_Health>();
            healthHandler.OnTakeDamage(damage);
        }
    }
}
