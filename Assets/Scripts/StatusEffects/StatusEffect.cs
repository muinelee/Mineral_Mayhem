using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    [SerializeField] public float duration = 3.0f;
    [SerializeField] public float tickRate = 0.2f;

    public abstract void OnStatusApplied(StatusHandler handler);
    public abstract void OnStatusUpdate(StatusHandler handler);
    public abstract void OnStatusEnded(StatusHandler handler);
}
