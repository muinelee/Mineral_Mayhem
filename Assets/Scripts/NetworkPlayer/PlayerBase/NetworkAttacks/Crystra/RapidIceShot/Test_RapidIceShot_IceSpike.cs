using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Fusion.NetworkCharacterController;

public class Test_RapidIceShot_IceSpike : NetworkAttack_Base
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    private float lifeTimer = 0;
    [SerializeField] private float offset;

    //components for attacking
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private int radius;

    private void Start()
    {
        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(0, offset);

        Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

        transform.Translate(offsetVector);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // move
        transform.Translate(Vector3.forward * speed);

        // manage lifetime
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifetime) Destroy(gameObject);

        DealDamage();
    }

    protected override void DealDamage() {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);


        for (int i = 0; i < hits.Count; i++) {
            Debug.Log($"Did we hit a hitbox? {hits[i].Hitbox}");
            NetworkPlayer_Health healthHandler = hits[i].GameObject.GetComponentInParent<NetworkPlayer_Health>();

            if (healthHandler) healthHandler.OnTakeDamage(damage);
            Runner.Despawn(GetComponent<NetworkObject>());
        }

    }
}
