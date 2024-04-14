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
        float moveDistance = speed * Time.deltaTime;

        if (distanceTravelled + moveDistance > maxTravelDistance)
        {
            moveDistance = maxTravelDistance - distanceTravelled;
        }

        transform.Translate(Vector3.forward * moveDistance);
        distanceTravelled += moveDistance;
    }
}
