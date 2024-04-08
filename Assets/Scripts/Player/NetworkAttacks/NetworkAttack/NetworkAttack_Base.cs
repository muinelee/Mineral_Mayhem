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
    [SerializeField] protected AudioClip SFX;
    [SerializeField] protected List<StatusEffect> statusEffectSO;

    protected virtual void DealDamage() { }
}