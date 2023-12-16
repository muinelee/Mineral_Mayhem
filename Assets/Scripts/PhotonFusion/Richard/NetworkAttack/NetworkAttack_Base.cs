using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkAttack_Base : NetworkBehaviour
{
    [Header("Name and Description")]
    public string attackName;
    public string attackDescription;

    [Header("Damage")]
    [SerializeField] private string damage;
}
