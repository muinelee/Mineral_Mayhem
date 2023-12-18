using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer_Attack : NetworkBehaviour
{
    // Control variables
    private bool canAttack = false;

    [Header("Q Attack Properties")]
    private NetworkObject qAttack;
    [SerializeField] private NetworkObject qAttackPrefab;
    [SerializeField] private float qAttackCoolDown;
    private float qAttackTimer;

    [Header("E Attack Properties")]
    [SerializeField] private GameObject eAttack;
    [SerializeField] private float eAttackCoolDown;
    private float eAttackTimer;

    //Components
    [SerializeField] private Animator anim;

    public override void Spawned()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isQAttack && !qAttack && !anim.GetBool("isAttacking"))
            {
                anim.SetBool("isAttacking", true);
                anim.CrossFade(qAttackPrefab.GetComponent<NetworkAttack_Base>().attackName, 0.2f);
            }
        }
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public void FireQAttack()
    {
        qAttack = Runner.Spawn(qAttackPrefab, transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }
}