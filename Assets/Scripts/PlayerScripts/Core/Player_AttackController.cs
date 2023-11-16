using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player_AttackController : NetworkBehaviour
{
    private bool canAttack = true;
    public Transform attackPoint;

    public Attack_Attribute currentAttack;

    public Attack_Attribute[] basicAttack;
    public int attackCounter =  0;
    public float basicAttackTimer = 100;

    public Attack_Attribute qAttack;
    public float qAttackTimer = 100;

    public Attack_Attribute eAttack;
    public float eAttackTimer = 100;

    public List<GameObject> attacks;

    private void Awake()
    {
        attacks.Add(eAttack.attackPrefab);
    }

    void Update()       // Should only manage timers
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.I)) CreateAttackServerRpc();

        if (basicAttackTimer < basicAttack[0].coolDown) basicAttackTimer += Time.deltaTime;
        if (qAttackTimer < qAttack.coolDown) qAttackTimer += Time.deltaTime;
        if (eAttackTimer < eAttack.coolDown) eAttackTimer += Time.deltaTime;
    }

    public void ActivateAttack(Attack_Attribute attack, ref float attackTimer)
    {
        if (attackTimer > attack.coolDown && canAttack)
        {
            currentAttack = attack;
            attackCounter++;
            canAttack = false;
            attackTimer = 0;
            attack.player = this.transform;
        }
    }

    public void PassCharge(float attackTimer)
    {
        currentAttack.TakeChargeDuration(attackTimer);
    }

    public void AttacksEnabled()
    {
        canAttack = true;
    }

    public void AttacksDisabled()
    {
        canAttack = false;
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public void ResetAttack()
    {
        attackCounter = 0;
        AttacksEnabled();
    }

    public void ReplaceAttack(ref Attack_Attribute equippedAttack, Attack_Attribute replacementAttack)
    {
        equippedAttack = replacementAttack;
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateAttackServerRpc()
    {
        Instantiate(eAttack.attackPrefab, attackPoint.position + eAttack.offset, transform.rotation).GetComponent<NetworkObject>().Spawn(true);
    }

    private void TestCallingAttackManagerAttack()
    {
        AttackManager.instance.TestFirePlayerAttack((int)OwnerClientId, 0);
    }
}