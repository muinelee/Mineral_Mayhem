using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnshackleBuff : NetworkAttack_Base
{
    // Variables to destroy this gameobject
    [SerializeField] private float lifetimeDuration;
    private TickTimer timer = TickTimer.None;

    // Forward offset during spawn
    [SerializeField] private float offset;

    // Components for getting objects in attack range
    //Use this to get the spawning player??
    [SerializeField] private float radius;
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;

    [SerializeField] GameObject sphere;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        timer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
        transform.position += transform.forward * offset;

        //Do I need the void? IEnumerator instead maybe, check how to get hit player
        StartCoroutine(ApplyBuff());

        Debug.Log("Unshackle activated");
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (timer.Expired(Runner))
        {
            timer = TickTimer.None;
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }

    IEnumerator ApplyBuff()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer);

        Debug.Log("Ability Called");

        for (int i = 0; i < hits.Count; i++)
        {
            Debug.Log($"Did we hit a hitbox? {hits[i].Hitbox}");

            CharacterEntity character = hits[i].GameObject.GetComponentInParent<CharacterEntity>();
            
            if (character)
            {
                character.OnCleanse();

                if (statusEffectSO.Count > 0 && character)
                {
                    foreach (StatusEffect status in statusEffectSO)
                    {
                        character.OnStatusBegin(status);
                    }
                }
            }

            yield return new WaitForSeconds(5);
            /*NetworkPlayer_Health healthHandler = hits[i].GameObject.GetComponentInParent<NetworkPlayer_Health>();

            if (healthHandler)
            {
                healthHandler.dmgReduction = 0.7f;
                Debug.Log("Applyingbuff");

                healthHandler.dmgReduction = 1.0f;
                Debug.Log("BuffEnded");
            }*/
        }
    }
}