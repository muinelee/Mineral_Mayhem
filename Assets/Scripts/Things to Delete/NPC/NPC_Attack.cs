using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Attack : MonoBehaviour
{
    public bool canAttack = true;
    public Player_Core target;      // Received from NPC_AINavigation

    [SerializeField] private int damage;
    [SerializeField] private int knockback;

    [SerializeField] private float aggroCoolDown;
    private float attackTimer = 100;

    void Update()
    {
        if (attackTimer < aggroCoolDown) attackTimer += Time.deltaTime;
        else if (attackTimer > aggroCoolDown && !canAttack) canAttack = true;
    }

    private void OnCollisionStay(Collision other) 
    {
        if (!target) return;    // If behavior is Wander

        if (other.transform.tag == "Participant" && canAttack) 
        {
            target.TakeDamage(damage, knockback, transform.position, STATUS_EFFECT.NONE, 0, 0);
            AggroReset();
        }
    }

    public void AggroReset()
    {
        canAttack = false;
        attackTimer = 0;
    }
}
