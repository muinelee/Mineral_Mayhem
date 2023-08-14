using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefense : MonoBehaviour
{
    private bool isBlocking;
    private bool isPerfectBlock;

    private float statusEffectTimer;
    private float statusEffectDuration;

    // private PlayerStats ps;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        isBlocking = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (statusEffectTimer > 0) statusEffectTimer -= Time.deltaTime;
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
            Debug.Log("Player takes " + damage + "damage");
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

    public void Knockback(float value, Vector3 attackSource)
    {

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
