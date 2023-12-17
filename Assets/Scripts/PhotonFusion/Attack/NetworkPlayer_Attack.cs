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

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isQAttack && !qAttack)
            {
                qAttack = Runner.Spawn(qAttackPrefab, transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
            }
        }
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }
}