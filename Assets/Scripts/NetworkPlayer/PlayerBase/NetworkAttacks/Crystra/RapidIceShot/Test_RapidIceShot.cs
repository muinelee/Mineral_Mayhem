using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_RapidIceShot : NetworkBehaviour
{
    [Header("Projectile Prefab")]
    [SerializeField] private NetworkObject iceSpikePF;

    [Header("Attack Properties")]
    [SerializeField] private int spikeNumber;
    [SerializeField] private float duration;
    private float frequency;
    private float attackTimer = 0;

    public override void Spawned()
    {
        frequency = duration / (float)spikeNumber;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer > frequency)
        {
            Instantiate(iceSpikePF, transform.position, transform.rotation);
            duration -= frequency;
        }

        if (duration <= 0) Runner.Despawn(GetComponent<NetworkObject>());
    }
}
