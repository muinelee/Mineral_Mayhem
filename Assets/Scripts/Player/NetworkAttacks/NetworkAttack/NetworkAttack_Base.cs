using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class NetworkAttack_Base : NetworkBehaviour
{
    // Source of attack
    //private PlayerRef playerRef;

    [Header("Base Attack Properties")]
    [SerializeField] protected int damage;
    [SerializeField] protected float knockback;
    [SerializeField] protected AudioClip SFX;
    [SerializeField] protected List<StatusEffect> statusEffectSO;
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