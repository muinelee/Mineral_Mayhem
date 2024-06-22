using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UIElements;

public abstract class NetworkAttack_Base : NetworkBehaviour
{
    // Source of attack
    //private PlayerRef playerRef;

    [Header("Base Attack Properties")]
    [SerializeField] protected float damage;
    [SerializeField] protected float knockback;
    [SerializeField] protected AudioClip[] SFX;
    [SerializeField] protected List<StatusEffect> statusEffectSO;
    [SerializeField] protected GameObject onHitEffect;
    [SerializeField] protected Vector3 onHitOffset;
    protected NetworkPlayer.Team thisTeam;

    public override void Spawned()
    {
        base.Spawned();

        if (!Object.HasStateAuthority) return;

        foreach (NetworkPlayer player in NetworkPlayer.Players)
        {
            if (player.Object.InputAuthority == Object.InputAuthority)
            {
                thisTeam = player.team;
                break;
            }
        }
    }

    protected virtual void DealDamage() { }

    protected bool CheckIfSameTeam(NetworkPlayer.Team team)
    {
        if (team == thisTeam) return true;

        return false;
    }
}