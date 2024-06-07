using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    private Transform rigTransform;

    public override void Spawned()
    {
        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);
        
        if (!Object.HasStateAuthority) return;

        timer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);
        transform.position += transform.forward * offset;

        //Do I need the void? IEnumerator instead maybe, check how to get hit player
        ApplyBuff();
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

    private void ApplyBuff()
    {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, collisionLayer);
        for (int i = 0; i < hits.Count; i++)
        {
            CharacterEntity character = hits[i].GameObject.GetComponentInParent<CharacterEntity>();
            rigTransform = hits[i].GameObject.transform.GetChild(0);
            this.transform.SetParent(rigTransform.transform);
            
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
        }
    }
}