using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_AnimationLink : NetworkBehaviour
{
    private NetworkPlayer_Movement playerMovement;
    private NetworkPlayer_Attack playerAttack;
    private Animator anim;

    // Start is called before the first frame update
    public override void Spawned()
    {
        playerMovement = GetComponentInParent<NetworkPlayer_Movement>();
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

    public void ResetAnimation()
    {
        playerAttack.ResetAttackCapabilities();
        anim.CrossFade("Run", 0.2f);
    }
}
