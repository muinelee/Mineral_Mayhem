using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Projectile : Attack
{
    [SerializeField] private float flightSpd;
    [SerializeField] private float range;
    private Rigidbody rb;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb) rb = gameObject.GetComponent<Rigidbody>();
        Vector3 direction = transform.rotation * Vector3.forward;
        rb.velocity = direction * flightSpd;

        startPos = transform.position;
    }

    private void Update() {
        if (Vector3.Distance(transform.position, startPos) > range) Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("NPC")) other.gameObject.GetComponent<NPC_Core>().TakeDamage(attackDamage, attackKnockback, playerTransform.position, STATUS_EFFECT.NONE, 0, 0);
        Destroy(this.gameObject);
    }
}
