using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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

    public void ReplenishEnergy(float amount)
    {
        // Replenish the player's energy by the specified amount
        // For example:
        // energy += amount;
        // You need to adapt this line based on how you implement energy management in your game.
    }
}