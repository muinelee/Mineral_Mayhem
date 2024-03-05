using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float speed;
    public float fireRate; 
    public Rigidbody rb; 


    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }
    void Update()
    {
        rb.transform.Rotate(Vector3.forward, 5);
        if(speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime); 
        }
        else
        {
            Debug.Log("No Speed");
        }
    }
    void OnCollisionEnter(Collision co)
    {
        if(co.gameObject.tag=="Cube")

        speed = 0; 
        Destroy (this.gameObject);
        Destroy (co.gameObject); 
    }
}
