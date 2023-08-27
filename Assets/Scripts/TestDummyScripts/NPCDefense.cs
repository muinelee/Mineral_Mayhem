using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDefense : MonoBehaviour
{
    private bool isBlocking = false;
    private bool isPerfectBlock = false;

    private float statusEffectTimer;
    private float statusEffectDuration;

    private NPCStats npcStats;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        npcStats = gameObject.GetComponent<NPCStats>();
        rb = gameObject.GetComponent<Rigidbody>();
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
            npcStats.health -= damage;
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
}
