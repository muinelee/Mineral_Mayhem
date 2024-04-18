using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Scriptable Object/Status Effect/Stun Status")]
public class StunStatus : StatusEffect
{
    [SerializeField] public string animationName = "GravityStun";
    public override void OnStatusApplied(StatusHandler handler)
    {
        if (animationName != "") handler.Character.Animator.anim.CrossFade(animationName, 0.2f);
        else handler.Character.Animator.ResetAnimation();
        handler.Character.Movement.canMove = false;
        handler.Character.Attack.canAttack = false;
        handler.Character.OnBlock(false);        
    }

    public override void OnStatusEnded(StatusHandler handler)
    {        
        handler.stun--;
        if (handler.stun < 1)
        {            
            handler.Character.Movement.canMove = true;
            handler.Character.Attack.canAttack = true;
            handler.Character.Animator.ResetAnimation();
        }
    }

    public override void OnStatusUpdate(StatusHandler handler)
    {
        handler.stun++;
    }

    public override void OnStatusCleansed(StatusHandler handler)
    {
        handler.stun = 0;

        handler.Character.Movement.canMove = true;
        handler.Character.Attack.canAttack = true;
        handler.Character.Animator.ResetAnimation();
    }
}
