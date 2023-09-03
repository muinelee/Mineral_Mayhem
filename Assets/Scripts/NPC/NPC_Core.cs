using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Core : MonoBehaviour
{
    [SerializeField] private float maxHP;
    private float currentHP;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, float knockback, Vector3 attackSource, STATUS_EFFECT statuseffect, float statusEffect_Value, float statusEffect_Duration)
    {
        currentHP -= damage;
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            Debug.Log("NPC dieded");
        }

        Vector3 direction = (transform.position - attackSource).normalized;
        Knockback(knockback, direction);
    }

    public void Knockback(float knockbackValue, Vector3 direction)
    {
        rb.velocity = direction * knockbackValue;
    }
}
