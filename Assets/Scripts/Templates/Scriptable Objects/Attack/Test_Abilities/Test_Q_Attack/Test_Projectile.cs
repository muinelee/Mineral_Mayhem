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
    protected override void OnEnable()
    {
        base.OnEnable();

        if (!rb) rb = gameObject.GetComponent<Rigidbody>();
        Vector3 direction = transform.rotation * Vector3.forward;
        rb.velocity = direction * flightSpd;

        startPos = transform.position;
    }

    private void Update() {
        if (Vector3.Distance(transform.position, startPos) > range) gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other) 
    {
        DealDamage(other.gameObject);
        gameObject.SetActive(false);
    }
}
