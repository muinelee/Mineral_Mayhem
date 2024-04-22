using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
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
    public const uint ButtonBlock = 1 << 9;

    public uint Buttons;
    public Vector2 cursorLocation;

    public bool IsDown(uint button) => (Buttons & button) == button;
}
