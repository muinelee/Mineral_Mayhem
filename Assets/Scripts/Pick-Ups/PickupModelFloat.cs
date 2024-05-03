using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupModelFloat : MonoBehaviour
{
    public float hoverSpeed = 1.0f;
    public float rotateSpeed = 15.0f;
    public float distance = 1.0f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {   
        //Floating
        float Yupdate = initialPosition.y + Mathf.Sin(Time.time * hoverSpeed) * distance;
        transform.position = new Vector3(transform.position.x, Yupdate, transform.position.z);

        //Rotating
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
