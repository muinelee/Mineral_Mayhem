using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupModelFloat : MonoBehaviour
{
    public float speed = 1.0f;
    public float distance = 1.0f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float Yupdate = initialPosition.y + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(transform.position.x, Yupdate, transform.position.z);
    }
}
