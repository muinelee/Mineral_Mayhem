using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Combo : MonoBehaviour
{
    public Attack_Attribute attack;

    private bool processingAttack = false;
    private bool isComboActive = false;
    private bool canChainCombo;
    private int comboStep = 0;
    private Coroutine comboWindowCoroutine;

    public LayerMask enemyLayer;
    Vector3 offset;
    public float attackRange = 2f;
    public float radius = 2f;
    public float damage;



    public void HandleCombo()
    {
        Debug.Log("HandleCombo");
        if (processingAttack)
        {
            return;
        }
        processingAttack = true;
        if (comboStep == 0)
        {
            isComboActive = true;
            comboStep = 1;
            canChainCombo = true;
            comboWindowCoroutine = StartCoroutine(ComboWindow());
            return;
        }
        else if (comboStep == 1 && canChainCombo)
        {
            if (comboWindowCoroutine != null)
            {
                StopCoroutine(comboWindowCoroutine);
            }
            comboStep = 2;
            canChainCombo = true;
            comboWindowCoroutine = StartCoroutine(ComboWindow());
            return;
        }
        else if (comboStep == 2 && canChainCombo)
        {
            if (comboWindowCoroutine != null)
            {
                StopCoroutine(comboWindowCoroutine);
            }
            canChainCombo = true;
            comboWindowCoroutine = StartCoroutine(ComboWindow());
            return;
        }
        
        processingAttack = false;
    }

    public void EndCombo()
    {
        Debug.Log("EndCombo");
        comboStep = 0;
        isComboActive = false;
        canChainCombo = false;
        processingAttack = false;
    }

    public IEnumerator ComboWindow()
    {
        yield return new WaitForSeconds(2f);
        {
            Debug.Log("Combo Over!");
            EndCombo();
        }
    }

    public void DetectHits()
    {
        damage = attack.damage;
        Collider[] hits = Physics.OverlapSphere(transform.position + offset, radius, enemyLayer);
        foreach (Collider hit in hits)
        {
            GameObject enemy = hit.gameObject;
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit: " + hit.name);
                hit.GetComponent<Player_Core>().TakeDamage(damage, damage, transform.position, STATUS_EFFECT.NONE, 0, 0);
            }
        }
    }


}
