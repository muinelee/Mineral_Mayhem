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
    public NetworkBool isBasicAttack;

    public NetworkBool canAttack;
    public NetworkBool canMove;
}

public struct NetworkInputStuff : INetworkInput
{
    public const uint ButtonDash = 1 << 0;
    public const uint ButtonQ = 1 << 1;
    public const uint ButtonE = 1 << 2;
    public const uint ButtonF = 1 << 3;
    public const uint ButtonBasic = 1 << 4;
    public const uint ButtonW = 1 << 5;
    public const uint ButtonA = 1 << 6;
    public const uint ButtonS = 1 << 7;
    public const uint ButtonD = 1 << 8;

    public uint Buttons;
    public Vector2 cursorLocation;

    public bool IsDown(uint button) => (Buttons * button) == button;
    public bool IsUp(uint button) => IsDown(button) == false;
}
