using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkAttack_Base : NetworkBehaviour
{
    // Source of attack
    private PlayerRef playerRef;

    [Header("Damage")]
    [SerializeField] protected int damage;

    [Header("SFX")]
    [SerializeField] private AudioClip sfx;
}