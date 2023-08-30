using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackController : MonoBehaviour
{
    [SerializeField] private bool canAttack;
    [SerializeField] private GameObject attackPoint;

    public Attack_Attribute qAttack;
    public bool qActive = false;
    public float qAttackTimer = 100;

    public Attack_Attribute eAttack;
    public float eAttackTimer = 100;

    public Attack_Attribute combo;
    public bool comboActive = false;
    public float comboTimer = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (qAttackTimer < qAttack.coolDown) qAttackTimer += Time.deltaTime;
        if (eAttackTimer < eAttack.coolDown) eAttackTimer += Time.deltaTime;
        if (comboTimer < combo.coolDown) comboTimer += Time.deltaTime;
    }

    public void ActivateAttack(Attack_Attribute attack, ref float attackTimer)
    {
        if (attackTimer > attack.coolDown)
        {
            attack.Activate(attackPoint.transform, transform.rotation);
            attackTimer = 0;
        }
    }

    public void ReplaceAttack(Attack_Attribute equippedAttack, Attack_Attribute replacementAttack)
    {
        equippedAttack = replacementAttack;
    }

    public void ActivateQAbility()
    {
        qAttack.Activate(attackPoint.transform, transform.rotation);
    }

    public void Awake()
    {
        //if (!attackPoint) attackPoint = GameObject.FindGameObjectWithTag("AttackPoint");    
    }

    public void ActivateComboAttack()
    {
        combo.Activate(attackPoint.transform, transform.rotation);
        
    }
}