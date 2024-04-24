using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupThrow : MonoBehaviour
{
    private Rigidbody rb;
    public float throwForce = 5f;
    public float yStop = 0.1f; //Point pickup stops
    private bool objectStop = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Throw();
    }

    void Update()
    {
        if (transform.position.y <= yStop && objectStop == false)
        {
            Debug.Log("Spawn Model");
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            Debug.Log("Gravity Deactivate");
            objectStop = true;
        }
    }

    void Throw()
    {   
        //Calculate Throw Direction
        Vector3 throwDir = transform.forward + Vector3.up;

        rb.AddForce(throwDir * throwForce, ForceMode.Impulse);
        
    }
}
