using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Stop Status")]
public class StunStatus : StatusEffect
{
    public override void OnStatusApplied(StatusHandler handler)
    {
        handler.controlState |= ControlState.Stun;
    }

    public override void OnStatusEnded(StatusHandler handler)
    {
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {
        handler.controlState |= ControlState.Stun;
    }
}
