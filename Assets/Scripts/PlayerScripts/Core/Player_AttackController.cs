using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackController : MonoBehaviour
{
    [SerializeField] private bool canAttack;
    [SerializeField] private Transform attackPoint;

    public Attack_Attribute qAttack;
    public float qAttackTimer = 100;

    public Attack_Attribute eAttack;
    public float eAttackTimer = 100;

    // Update is called once per frame
    void Update()
    {
        if (qAttackTimer < qAttack.coolDown) qAttackTimer += Time.deltaTime;
        if (eAttackTimer < eAttack.coolDown) eAttackTimer += Time.deltaTime;
    }

    public void ActivateAttack(Attack_Attribute attack, ref float attackTimer)
    {
        if (attackTimer > attack.coolDown)
        {
            attack.Activate(attackPoint, transform.rotation);
            attackTimer = 0;
        }
    }
}