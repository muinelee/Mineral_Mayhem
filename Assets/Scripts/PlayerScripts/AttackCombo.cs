using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCombo : MonoBehaviour
{
    private float comboWindow = 0.5f;
    private int combo = 0;
    private HitDetection hitDetection;
    private bool nextCombo = false;
    private bool canAttack = true;

    private void Start()
    {
        hitDetection = GetComponent<HitDetection>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttack();
        }
    }

    public void StartAttack()
    {
        if (!canAttack) return;

        Debug.Log("Attack initiated");

        if (combo == 0)
        {
            Attack1();           
        }
        else if (combo == 1)
        {
            if (nextCombo)
            { 
                Attack2();
            }
            else
            {
                ResetCombo();
            }
        }
        else if (combo == 2)
        {
            if (nextCombo)
            {
                Attack3();
            }
            else
            {
                ResetCombo();
            }
        }
        else if (combo == 3)
        {
            ResetCombo();
        }

        nextCombo = false;
        StartCoroutine(WaitForCombo());
        StartCoroutine(AttackDelay());
    }

    private void Attack1()
    {
        Debug.Log("Attack 1");
        hitDetection.DetectHits();
        combo++;
    }

    private void Attack2()
    {
        Debug.Log("Attack 2");
        hitDetection.DetectHits();
        combo++;
    }

    private void Attack3()
    {
        Debug.Log("Attack 3");
        hitDetection.DetectHits();
        combo = 3;
    }

    private IEnumerator WaitForCombo()
    { 
        yield return new WaitForSeconds(comboWindow);
        nextCombo = true;
    }

    private IEnumerator AttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }

    private IEnumerator ComboCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }

    public void ResetCombo()
    {
        combo = 0;
        ComboCooldown();
        Debug.Log("Combo reset");
    }
}
