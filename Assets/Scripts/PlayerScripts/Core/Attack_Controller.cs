using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Controller : MonoBehaviour
{
    [SerializeField] private bool canAttack;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private Attack_Attribute qAttack;
    public float qAttackTimer = 100;

    [SerializeField] private Attack_Attribute eAttack;
    public float eAttackTimer = 100;

    // Update is called once per frame
    void Update()
    {
        if (qAttackTimer < qAttack.coolDown) qAttackTimer += Time.deltaTime;
        if (eAttackTimer < eAttack.coolDown) eAttackTimer += Time.deltaTime;
    }

    public void ActivateQ()
    {
        if (qAttackTimer > qAttack.coolDown)
        {
            qAttack.Activate(attackPoint, transform.rotation);
            qAttackTimer = 0;
        } 
    }

    public void ActivateE()
    {
         if (eAttackTimer > eAttack.coolDown)
        {
            eAttack.Activate(attackPoint, transform.rotation);
            eAttackTimer = 0;
        } 
    }
}