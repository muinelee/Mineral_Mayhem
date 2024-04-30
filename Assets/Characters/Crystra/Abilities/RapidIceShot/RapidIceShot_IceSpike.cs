using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


    //attack indexing
    private static int attackIndex = 0;

    //spawn indexing
    private static int spawnIndex = 0;

    public override void Spawned()
    {
        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);

        transform.position += Vector3.up * spawnHeight;
        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(0, offset);

        Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

        GetComponent<NetworkRigidbody>().Rigidbody.velocity = transform.forward * speed;

        //track the spawns
        TrackSpawns();
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        // move
        transform.Translate(Vector3.forward * speed);

        // manage lifetime
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifetime) Runner.Despawn(Object);

        DealDamage();
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
    

    protected override void DealDamage() {

        if (!Runner.IsServer) return;
        
        float totalDamage = 0;
        //hit signature to check 
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        //loop to check for hits
        for (int i = 0; i < hits.Count; i++) {
            IHealthComponent healthComponent = hits[i].GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null) {
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
                healthComponent.OnTakeDamage((int)totalDamage);
                TrackAttacks();

                //debug attack index
                //Debug.Log($"Attack Index: {attackIndex}");
                //debug damage multiplier
                //Debug.Log($"Damage Multiplier: {damageMultiplier}");
                //debug log to check if damage is being dealt
                //Debug.Log($"Dealt {totalDamage} damage to {healthHandler.gameObject.name}");

                Runner.Despawn(GetComponent<NetworkObject>());
            }
        }
    }
} 
