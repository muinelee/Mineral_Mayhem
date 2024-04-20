using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreCollecibleSpawnMovement : MonoBehaviour
{
    public float radius = 2f; //Radius of movement
    public float speed = 2f; //speed of movement

    private Vector3 centerPoint; //Center of circular movement
    private float angle = 0f;

    void Start()
    {
        centerPoint = transform.position;
    }

    void Update()
    {
        angle += speed * Time.deltaTime;

        //New position 
        float x = centerPoint.x + Mathf.Cos(angle) * radius;
        float y = centerPoint.y; //No current need to move along y axis
        float z = centerPoint.z + Mathf.Sin(angle) * radius;

        //Update position
        transform.position = new Vector3(x, y, z);
    }
}
