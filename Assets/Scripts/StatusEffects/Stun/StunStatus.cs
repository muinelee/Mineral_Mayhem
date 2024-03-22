using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Stun Status")]
public class StunStatus : StatusEffect
{
    public override void OnStatusApplied(StatusHandler handler)
    {
        handler.Character.Movement.canMove = false;
        handler.Character.Attack.canAttack = false;
    }

    public override void OnStatusEnded(StatusHandler handler)
    {        
        if (handler.CheckIfStunned())
        {            
            handler.Character.Movement.canMove = true;
            handler.Character.Attack.canAttack = true;
        }
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {
        handler.stun += 1;
        Debug.Log(handler.stun);
    }
}
