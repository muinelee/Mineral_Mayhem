using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_AnimationLink : CharacterComponent
{
    private NetworkPlayer_Attack playerAttack;
    public Animator anim;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetAnimationLink(this);
    }

    // Start is called before the first frame update
    public override void Spawned()
    {
        playerAttack = GetComponentInParent<NetworkPlayer_Attack>();
        anim = GetComponent<Animator>();
    }

    public void FireQAttack()
    {
        playerAttack.FireQAttack();
    }

    public void FireEAttack()
    {
        playerAttack.FireEAttack();
    }

    public void FireFAttack()
    {
        playerAttack.FireFAttack();
    }

    public void FireBasicAttack()
    {
        playerAttack.FireBasicAttack();
    }

    public void AllowChainBasicAttack()
    {
        playerAttack.AllowChainBasicAttack();
    }

    public void ChainBasicAttack()
    {
        playerAttack.ChainBasicAttack();
    }

    public void FireBlock()
    {
        playerAttack.FireBlock();
    }

    public void ResetAnimation()
    {
        playerAttack.ResetAttackCapabilities();
        anim.CrossFade("Run", 0.2f);
    }
}
