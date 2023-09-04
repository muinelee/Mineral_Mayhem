using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Core : MonoBehaviour
{
    [SerializeField] public float currentHP;
    [SerializeField] public float heroMaxHP;

    private Rigidbody rb;
    [SerializeField] private CapsuleCollider cc;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!cc) cc = GetComponent<CapsuleCollider>();
        if (!anim) anim = GetComponentInChildren<Animator>();
        //npcStats.currentHP = npcStats.maxHP;
    }

    public void Awake()
    {
        currentHP = heroMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, float knockback, Vector3 attackSource, STATUS_EFFECT statuseffect, float statusEffect_Value, float statusEffect_Duration)
    {
        currentHP -= damage;
        anim.CrossFade("DummyPushed", 0.3f);
        Debug.Log(currentHP);

        if (currentHP <= 0)
        {
            Death();            
        }

        Vector3 direction = (transform.position - attackSource).normalized;
        Knockback(knockback, direction);
    }

    public void Knockback(float knockbackValue, Vector3 direction)
    {
        rb.velocity = direction * knockbackValue;
    }

    public void ResetAnimation()
    {
        anim.CrossFade("DummyIdle", 0.3f);
    }

    public void Death()
    {
        Debug.Log("NPC has Dieded");
        cc.enabled = false;
        rb.isKinematic = true;
        anim.CrossFade("DummyDied", 0.3f);
        Destroy(gameObject, 3f);
    }
}
