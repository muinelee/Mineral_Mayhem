using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Slow Status")]
public class SlowStatus : StatusEffect
{
    [SerializeField, Range(0f, Mathf.Infinity)] private float slowAmount = 2f;
    public override void OnStatusApplied(StatusHandler handler)
    {
        handler.speed.AddModifier(-slowAmount);
    }

    public override void OnStatusEnded(StatusHandler handler)
    {
        handler.speed.RemoveModifier(-slowAmount);
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {

    }
    public override void OnStatusCleansed(StatusHandler handler)
    {
        handler.speed.RemoveModifier(-slowAmount);
    }
}
