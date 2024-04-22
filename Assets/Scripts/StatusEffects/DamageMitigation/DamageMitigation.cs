using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Damage Mitigation")]
public class DamageMitigation : StatusEffect
{
    [SerializeField] private float damageMitigation = 25;

    public override void OnStatusApplied(StatusHandler handler)
    {
        handler.armor.AddModifier(damageMitigation);
    }

    public override void OnStatusEnded(StatusHandler handler)
    {
        handler.armor.RemoveModifier(damageMitigation);
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {
    }

    public override void OnStatusCleansed(StatusHandler handler)
    {
        //handler.armor.RemoveModifier(damageMitigation);
    }
}
