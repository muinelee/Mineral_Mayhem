using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Damage Over Time")]
public class DamageOverTimeStatus : StatusEffect
{
    [SerializeField] private int damage = 2;

    public override void OnStatusApplied(StatusHandler handler)
    {
        
    }

    public override void OnStatusEnded(StatusHandler handler)
    {
        
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {
        //NetworkPlayer_Health.HP.OnTakeDamage(damage);
    }
}
