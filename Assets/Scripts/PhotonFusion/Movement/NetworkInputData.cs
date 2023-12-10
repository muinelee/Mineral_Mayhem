using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveDirection;
    public float lookDirection;
    public NetworkBool isDashing;
    public NetworkBool isQAttack;
    public NetworkBool isEAttack;
}
