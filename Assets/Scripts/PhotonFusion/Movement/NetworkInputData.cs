using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveDirection;
    public Vector2 lookDirection;
    public bool isDashing;
    public bool isQAttack;
    public bool isEAttack;
}
