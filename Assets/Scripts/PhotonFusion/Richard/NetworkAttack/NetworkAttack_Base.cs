using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkAttack_Base : NetworkBehaviour
{
    // Source of attack
    private PlayerRef playerRef;

    [Header("Name and Description")]
    public string attackName;
    public string attackDescription;

    [Header("Damage")]
    [SerializeField] private string damage;

    [Header("SFX")]
    [SerializeField] private AudioClip sfx;
}
