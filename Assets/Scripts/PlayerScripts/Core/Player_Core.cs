using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_InputController), typeof(Player_Movement), typeof(Player_AttackController))]

public class Player_Core : MonoBehaviour
{
    private float _hp = 50;
    public float hp
    {
        get 
        {
            return _hp;
        }
        
        set 
        {
            _hp = value;
            if (_hp <= 0) pi.enabled = false;
        }
    }

    private bool isBlocking = false;
    private bool isPerfectBlock = false;

    private float statusEffectTimer;
    private float statusEffectDuration;

    private Rigidbody rb;
    private Player_InputController pi;

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (statusEffectTimer > 0) statusEffectTimer -= Time.deltaTime;
    }


    public void TakeDamage(float damage, float knockback, Vector3 attackSource, STATUS_EFFECT statusEffect, float statusEffect_Value, float statusEffect_Duration)
    {
        // ----- DAMAGE STEP -----

        if (isPerfectBlock)
        {
            // Insert perfect block code
            return;
        }

        if (isBlocking) damage *= 0.2f;

        else if (!isBlocking)
        {
            hp -= damage;
            rb.velocity = Vector3.zero;
            Vector3 direction = (transform.position - attackSource).normalized;
            Knockback(knockback, direction);
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
