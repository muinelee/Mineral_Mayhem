using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FrostCloud : NetworkAttack_Base
{
    [Header("Movement Properties")]
    [SerializeField] private float lifetime;
    [SerializeField] private float offset;

    [Header("Lifetime Components")]
    private TickTimer lifeTimer = TickTimer.None;

    [Header("Other Attack Properties")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    [Header("VFX Properties")]
    [SerializeField] private GameObject attackVFX;

    public override void Spawned()
    {
        base.Spawned();
         
        Instantiate(attackVFX, this.transform.position, Quaternion.identity);
        AudioManager.Instance.PlayAudioSFX(SFX[0], transform.position);
        //AudioManager.Instance.PlayAudioSFX(SFX[1], transform.position);

        transform.position += transform.up * offset;

        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
        {
            AttackEnd();
            return;
        }

        DealDamage();
    }

    public void AttackEnd()
    {
        lifeTimer = TickTimer.None;
        Runner.Despawn(Object);
    }

    protected override void DealDamage()
    {
        if (!Runner) return;

        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
        foreach (LagCompensatedHit hit in hits)
        {
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null)
            {
                Debug.Log("Frost Cloud Deal Damage is being called");
                if (healthComponent.isDead || CheckIfSameTeam(healthComponent.GetTeam())) continue;

                healthComponent.OnTakeDamage(damage, false);
            }
            
            CharacterEntity characterEntity = hit.GameObject.GetComponentInParent<CharacterEntity>();

            if (statusEffectSO.Count > 0 && characterEntity)
            {
                foreach (StatusEffect status in statusEffectSO)
                {
                    characterEntity.OnStatusBegin(status);
                }
            }
        }
    }
}
