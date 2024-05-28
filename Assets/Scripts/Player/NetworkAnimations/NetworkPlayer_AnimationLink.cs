using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_AnimationLink : CharacterComponent
{
    public Animator anim;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetAnimationLink(this);
    }

    // Start is called before the first frame update
    public override void Spawned()
    {
        anim = GetComponent<Animator>();        
    }

    public void FireQAttack()
    {
        Character.Attack.FireQAttack();
    }

    public void FireEAttack()
    {
        Character.Attack.FireEAttack();
    }

    public void FireFAttack()
    {
        Character.Attack.FireFAttack();
    }

    public void FireBasicAttack()
    {
        Character.Attack.FireBasicAttack();
    }

    public void AllowChainBasicAttack()
    {
        Character.Attack.AllowChainBasicAttack();
    }

    public void ChainBasicAttack()
    {
        Character.Attack.ChainBasicAttack();
    }

    public void AllowMomentum()
    {
        Character.Attack.AttackMomentum();
    }

    public void ResetAnimation()
    {
        Character.Attack.ResetAttackCapabilities();
        anim.CrossFade("Run", 0.2f);
        anim.CrossFade("Run", 0.2f, 1);
    }

    public override void OnBlock(bool isBlocking)
    {
        base.OnBlock(isBlocking);

        if (isBlocking)
        {
            anim.CrossFade("Block", 0.08f);
            anim.CrossFade("Helper", 0.2f, 1);
        }
        else
        {
            if (Character.StatusHandler.IsStunned())
            {
                return;
            }
            ResetAnimation();
        }
    }
}
