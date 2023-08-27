using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefense : MonoBehaviour
{
    private bool isBlocking = false;
    private bool isPerfectBlock = false;

    private float statusEffectTimer;
    private float statusEffectDuration;

    private PlayerCore pCore;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        pCore = gameObject.GetComponent<PlayerCore>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (statusEffectTimer > 0) statusEffectTimer -= Time.deltaTime;

        // ----- Delete in final build -----
        if (Input.GetKeyDown(KeyCode.UpArrow)) TakeDamage(10, 20, transform.position + Vector3.forward, STATUS_EFFECT.NONE, 0,0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TakeDamage(10, 20, transform.position + Vector3.back, STATUS_EFFECT.NONE, 0,0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TakeDamage(10, 20, transform.position + Vector3.right, STATUS_EFFECT.NONE, 0,0);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TakeDamage(10, 20, transform.position + Vector3.left, STATUS_EFFECT.NONE, 0,0);
        // ----- Delete in final build -----

    }

    public void TakeDamage(float damage, float knockback, Vector3 attackSource, STATUS_EFFECT statusEffect, float statusEffect_Value, float statusEffect_Duration)
    {
        // ----- DAMAGE STEP -----

        if (isPerfectBlock)
        {
            Debug.Log("Insert perfect block code");
            return;
        }

        if (isBlocking) damage *= 0.2f;

        else if (!isBlocking)
        {
            pCore.hp -= damage;
            rb.velocity = Vector3.zero;
            Vector3 direction = (transform.position - attackSource).normalized;
            Knockback(knockback, direction);
        }


        // ----- STATUS EFFECTS -----

        switch (statusEffect)
        {
            case STATUS_EFFECT.BURN:
            {
                // Call burn effect
                break;
            }

            case STATUS_EFFECT.KNOCKUP:
            {
                // Call knock up effect
                break;
            }

            case STATUS_EFFECT.NONE:
            {
                // No effect
                break;
            }

            case STATUS_EFFECT.SLOW:
            {
                // Slow Effect
                break;
            }

            case STATUS_EFFECT.SNARE:
            {
                // Snare Effect
                break;
            }
            
            case STATUS_EFFECT.STUN:
            {
                // Stun Effect
                break;
            }
        }
    }

    public void SetIsBlocking(bool b)
    {
        isBlocking = b;                                 // Use animation event to turn blocking on/off
    }

    public void SetIsPerfectBlocking(bool b)
    {
        isPerfectBlock = b;                             // Use animation event to turn perfectblocking on/off
    }

    public void Knockback(float knockbackValue, Vector3 direction)
    {
        rb.velocity = direction * knockbackValue * Time.deltaTime * 100;
    }

    #region <----------STATUS EFFECT---------->

    private void Burn()
    {
        // Insert burn effect here
    }  

    private void Knockup()
    {
        // Insert knockup effect here
    }

    private void Slow()
    {
        // Insert Slow effect here
    }

    private void Snare()
    {
        // Insert Snare effect here
    }

    private void Stun()
    {
        // Insert Stun effect here
    }
    #endregion

}
