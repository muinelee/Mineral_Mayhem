using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Damage Over Time")]
public class DamageOverTimeStatus : StatusEffect
{
    [SerializeField] private float damage = 2;

    public override void OnStatusApplied(StatusHandler handler)
    {
        
    }

    public override void OnStatusEnded(StatusHandler handler)
    {
        
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {
        if (handler.Character) handler.Character.OnHit(damage);
        // Temporary solution for between refactoring
        else
        {
            handler.GetComponentInParent<NetworkPlayer_Health>().OnTakeDamage((int)damage);
        }
    }

    public override void OnStatusCleansed(StatusHandler handler)
    {
        
    }
}
