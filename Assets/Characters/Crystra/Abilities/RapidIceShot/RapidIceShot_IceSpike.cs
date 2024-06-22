using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class RapidIceShot_IceSpike : NetworkAttack_Base
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    private float lifeTimer = 0;
    [SerializeField] private float offset;
    [SerializeField] private float spawnHeight;

    // Components for getting objects in attack range
    [SerializeField] private float radius;
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer; 

    // Damage properties
    [Header("Damage Properties")]
    [SerializeField] private static float damageMultiplier = 1f;

    private NetworkRunner runner;

    //attack indexing
    private static int attackIndex = 0;

    //spawn indexing
    private static int spawnIndex = 0;

    public override void Spawned()
    {        
        base.Spawned();

        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);
        runner = FindAnyObjectByType<NetworkRunner>();
        //AudioManager.Instance.PlayAudioSFX(SFX[1], transform.position);

        if (!runner.IsServer) return;

        Vector3 offsetVector = new Vector3(Random.Range(-offset, offset), Random.Range(0, offset), Random.Range(-offset, offset));

        transform.position += offsetVector;
        GetComponent<NetworkRigidbody>().Rigidbody.velocity = transform.forward * speed;

        //track the spawns
        TrackSpawns();
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (!runner.IsServer) return;

        // move
        transform.Translate(Vector3.forward * speed);
        
        DealDamage();

        // manage lifetime
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifetime) runner.Despawn(Object);
    }

    private void TrackAttacks() {
        //add one to attack index
        attackIndex++;

        //if attack index is greater than 5, reset it to 0
        if (attackIndex >= 5) {
            attackIndex = 0;
        }
        //if damage multiplier is greater than 2, reset it to 1.2
        if (damageMultiplier >= 2) {
            damageMultiplier = 1f;
        }
    }

    //tracking the spawns of attacks, to reset attack index and multipler after 5 attacks
    private void TrackSpawns() {
        //add one to spawn index
        spawnIndex++;
        //if attack index is greater than 6, reset it to 1
        if (spawnIndex >= 6) {
            spawnIndex = 1;
            attackIndex = 0;
            damageMultiplier = 1f;
        }
        //a lil debug log to check if the spawn index is working
        //Debug.Log($"Spawn Index: {spawnIndex}");
    }
    

    protected override void DealDamage() {;

        if (!runner.IsServer) return;

        float totalDamage = 0;
        //hit signature to check 
        runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        //loop to check for hits
        for (int i = 0; i < hits.Count; i++) {
            IHealthComponent healthComponent = hits[i].GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null) {

                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                runner.Spawn(this.onHitEffect, this.transform.position + onHitOffset, Quaternion.identity);

                //if its the first hit, ignore the multiplier
                if (attackIndex < 1) {
                    //total damage equal to damage
                    totalDamage = damage;
                }
                if (attackIndex >= 1) {
                    //if not first hit, multiply the damage
                    damageMultiplier += 0.25f;
                    totalDamage = damage * damageMultiplier;
                }

                //send damage to health handler as int
                healthComponent.OnTakeDamage(totalDamage, true);
                TrackAttacks();

                //debug attack index
                //Debug.Log($"Attack Index: {attackIndex}");
                //debug damage multiplier
                //Debug.Log($"Damage Multiplier: {damageMultiplier}");
                //debug log to check if damage is being dealt
                //Debug.Log($"Dealt {totalDamage} damage to {healthHandler.gameObject.name}");

                runner.Despawn(Object);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (runner == null) return;

        if (!runner.IsServer) return;
        runner.Spawn(this.onHitEffect, this.transform.position + onHitOffset, Quaternion.identity);
        runner.Despawn(Object);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);
    }
} 
