using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 moveDirection;
    public Vector3 cursorLocation;
    public float lookDirection;
    public NetworkBool isDashing;
    public NetworkBool isQAttack;
    public NetworkBool isEAttack;
    public NetworkBool isFAttack;

    public NetworkBool canAttack;
    public NetworkBool canMove;
}
