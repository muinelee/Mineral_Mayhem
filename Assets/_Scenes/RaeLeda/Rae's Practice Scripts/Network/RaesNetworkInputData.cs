using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RaesNetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public float rotationInput;
    public NetworkBool isJumpPressed; 
}
