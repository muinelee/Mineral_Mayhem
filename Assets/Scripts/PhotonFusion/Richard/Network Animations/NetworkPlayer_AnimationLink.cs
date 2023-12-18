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

    public override void FixedUpdateNetwork()
    {

    }

    public void FireQAttack()
    {
        playerAttack.FireQAttack();
    }

    public void ResetAnimation()
    {
        anim.SetBool("isAttacking", false);
        anim.CrossFade("Run", 0.2f);
    }
}
