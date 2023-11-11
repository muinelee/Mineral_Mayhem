using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AttackGameMultiplayer : NetworkBehaviour
{
    public static AttackGameMultiplayer instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}
