using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Slow Status")]
public class SlowStatus : StatusEffect
{
    public override void OnStatusApplied(StatusHandler handler)
    {
        handler.speed.AddModifier(-2);
    }

    public override void OnStatusEnded(StatusHandler handler)
    {
        handler.speed.RemoveModifier(-2);
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {

    }
}
