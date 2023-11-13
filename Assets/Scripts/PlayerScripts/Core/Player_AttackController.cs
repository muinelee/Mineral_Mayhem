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
        CreateAttack(qAttack);
        CreateAttack(eAttack);
    }

    void Update()       // Should only manage timers
    {
        if (Input.GetKeyDown(KeyCode.I)) FireAttack(0);
        if (Input.GetKeyDown(KeyCode.O)) FireAttack(1);

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

    public void FireAttack(int index)
    {
        if (IsOwner)
        {
            GameObject attack = Instantiate(attacks[index], attackPoint.position, transform.rotation);
            attack.SetActive(true);
            attack.GetComponent<NetworkObject>().Spawn(true);
        }
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

    private void CreateAttack(Attack_Attribute attackSO)
    {
        GameObject attack = Instantiate(attackSO.attackPrefab, attackPoint.position + attackSO.offset, transform.rotation);
        attacks.Add(attack);
    }

    public void ReplaceAttack(ref Attack_Attribute equippedAttack, Attack_Attribute replacementAttack)
    {
        equippedAttack = replacementAttack;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActivateAttackServerRpc(int clientID, int index, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"Is the ClientID {serverRpcParams.Receive.SenderClientId} the owner? {IsOwner}");
    }
}