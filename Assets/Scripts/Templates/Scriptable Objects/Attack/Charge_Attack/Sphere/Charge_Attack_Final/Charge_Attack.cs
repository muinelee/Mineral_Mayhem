using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge_Attack : Attack
{
    [SerializeField] private Attack[] attackPrefabs;

    void Start()
    {
        if (holdDuration < 1) SelectChargeAttack(0);
        else if (holdDuration < 2) SelectChargeAttack(1);
        else SelectChargeAttack(2);
    }

    private void LateUpdate() 
    {
        Destroy(this.gameObject);
    }

    private void SelectChargeAttack(int index)
    {
        Attack chargeAttack = Instantiate(attackPrefabs[index], transform.position, transform.rotation);
        chargeAttack.playerTransform = this.playerTransform;
    }
}
