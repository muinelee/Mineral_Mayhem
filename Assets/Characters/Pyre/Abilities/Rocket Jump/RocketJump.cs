using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketJump : NetworkAttack_Base
{
    private Transform playerSource;

    [Header("Attack Properties")]
    [SerializeField] private float radius;    
    [SerializeField] private float forceForward;
    [SerializeField] private float forceUpward;
    [SerializeField] private float landingOffset;
    [SerializeField] private LayerMask collisionLayer;

    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    private NetworkRigidbody rb;
    [SerializeField] private float gravityScale;

    public override void Spawned()
    {
        base.Spawned();
        
        if (!Runner.IsServer) return;

        if (Runner) Debug.Log("We have a runner");

        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer);

        for (int i = 0; i < hits.Count; i++)
        {
            CharacterEntity character = hits[i].GameObject.GetComponentInParent<CharacterEntity>();

            if (character)
            {
                if (character.Object.InputAuthority != Object.InputAuthority)
                {
                    Debug.Log("This is not the player you're looking for");
                    continue;
                }

                playerSource = character.transform;
                rb = character.Rigidbody;
                rb.Rigidbody.AddForce(transform.forward * forceForward);
            }
        }
    }
}
